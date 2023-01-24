using System.Text.RegularExpressions;
using System.Xml.Linq;
using CelesteNyaNetBot.Response;
using SaladimQBot.Core;
using SaladimQBot.Extensions;
using SaladimQBot.GoCqHttp;
using SaladimQBot.Shared;

namespace CelesteNyaNetBot;

public class LoginModule : CommandModule
{
    public const string ResultHttpUrlFormat = @"http://localhost:38038/setkey?value={0}";

    protected ITokenService tokenService;
    protected Regex nameRegex = new(@"^[_a-zA-Z\^\.0-9]+$", RegexOptions.Compiled);
    protected MemorySessionService memorySessionService;
    protected CoroutineService coroutineService;

    public LoginModule(ITokenService tokenService, MemorySessionService memorySessionService, CoroutineService coroutineService)
    {
        this.tokenService = tokenService;
        this.memorySessionService = memorySessionService;
        this.coroutineService = coroutineService;
    }

    [Command("bind")]
    public void Bind(params string[] userNameStrs)
    {
        var userSession = memorySessionService.GetUserSession<BindStateSession>(Content.Executor.UserId);
        if (coroutineService.IsRunning(userSession.Enumerator))
        {
            return;
        }
        else
        {
            var e = GetBindCoroutine();
            coroutineService.AddNewCoroutine(e);
            userSession.Enumerator = e;
        }

        IEnumerator<EventWaiter> GetBindCoroutine()
        {
            long? groupIdWay = null;
            long userId;
            if (Content.Message is IGroupMessage groupMsg)
            {
                groupIdWay = groupMsg.Group.GroupId;
                userId = groupMsg.Sender.UserId;
            }
            else if (Content.Message is IPrivateMessage privateMessage)
            {
                groupIdWay = privateMessage.TempSourceGroupId;
                userId = privateMessage.Sender.UserId;
            }
            else
            {
                yield break;
            }
            string userName = string.Join(' ', userNameStrs);
            if (!nameRegex.IsMatch(userName))
            {
                Content.Message.Sender.SendMessageAsync(groupIdWay, "昵称不符合规则！只允许大小写字母和数字以及符号_和^和.的组合.");
                yield break;
            }
            try
            {
                Content.Message.Sender.SendMessageAsync(groupIdWay,
                    $"你确定要将你的id绑定到用户名 `{userName}` 上吗, 使用!bind {userName} confirm确认该操作."
                    );
            }
            //临时消息发送都会发生这个异常, 除了魔改或等go-cqhttp更新没办法
            catch (CqApiCallFailedException) { }

            var cmdWaiter = new CommandWaiter(
                Content.SimCommandExecuter,
                Content.Executor,
                "bind",
                new object[] { new string[] { userName, "confirm" } }
                );
            var tickWaiter = new TickWaiter(20);
            EventWaiter resultWaiter = null!;
            yield return new OrEventWaiter(cmdWaiter, tickWaiter, w => resultWaiter = w);
            if (ReferenceEquals(resultWaiter, tickWaiter))
            {
                Content.Executor.SendMessageAsync(groupIdWay, "确认超时, 请再次使用!bind指令进行绑定");
            }
            else if (ReferenceEquals(resultWaiter, cmdWaiter))
            {
                NayResponse? res = null; CreateAccountResponseData? data = null;
                try
                {
                    (res, data) = tokenService.CreateAccountAsync(Content.Executor.UserId, userName).Result;
                }
                catch (Exception)
                {
                    Content.Message.MessageWindow.SendTextMessageAsync("服务端异常");
                    yield break;
                }
                try
                {
                    if (res is null)
                    {
                        Content.Message.Sender.SendMessageAsync(groupIdWay, "内部异常");
                        yield break;
                    }
                    if (res.Code is 200)
                    {
                        string finalUrl = string.Format(ResultHttpUrlFormat, data!.Token);
                        if (groupIdWay is not null)
                            Content.Client.SetGroupCardAsync(groupIdWay.Value, Content.Executor.UserId, userName);
                        Content.Message.Sender.SendTextMessageAsync(
                            $"您已经成功绑定至群服,请遵守服务器游玩规则\r\n" +
                            $"客户端MOD下载:https://celeste.weg.fan/api/v2/download/mods/Miao.CelesteNet.Client\r\n" +
                            $"客户端下载由WGF提供 (特别感谢)\r\n" +
                            $"如何登陆? 请安装MOD后启动蔚蓝,并在游戏启动完毕后点击下面的连接\r\n" +
                            $"{finalUrl}\r\n" +
                            $"浏览器返回OK两个字符代表登陆完毕\r\n" +
                            $"尽情享受群服吧\r\n" +
                            $"注意 如果更换电脑或客户端请使用指令 !relogin重新登陆"
                            );
                        if (Content.Message is IGroupMessage groupMessage)
                        {
                            groupMessage.Sender.SetGroupCardAsync(userName);
                        }
                    }
                    else if (res.Code is 201)
                    {
                        Content.Message.Sender.SendMessageAsync(groupIdWay, $"绑定失败, 已经存在绑定的账号。如需重新登陆请使用 !relogin");
                    }
                    else if (res.Code is 401 or 403 or 500)
                    {
                        Content.Message.Sender.SendMessageAsync(groupIdWay, $"内部异常，消息为 {res.Code}:{res.Message}");
                    }
                }
                catch (CqApiCallFailedException) { }
                yield break;
            }
        }
    }

    [Command("relogin")]
    public void Relogin()
    {
        try
        {
            var (res, data) = tokenService.RequestAuthAsync(Content.Executor.UserId).Result;
            if (res is null)
            {
                Content.MessageWindow.SendTextMessageAsync("内部异常").ConfigureAwait(false);
                return;
            }
            if (res.Code is 200)
            {
                string finalUrl = string.Format(ResultHttpUrlFormat, data!.Token);
                if (Content.Message is IGroupMessage)
                    Content.Executor.SendTextMessageAsync("新的登录链接已生成！请查看私信消息，旧链接将会自动过期。");
                Content.Executor.SendTextMessageAsync($"这个是你的新链接哦，旧链接已自动失效:\n{finalUrl}");
            }
            else if (res.Code is 201)
            {
                Content.MessageWindow.SendTextMessageAsync($"生成失败，不存在已绑定的账号，请使用!bind 用户名进行绑定。");
            }
            else if (res.Code is 401 or 403 or 500)
            {
                Content.MessageWindow.SendTextMessageAsync($"内部异常，消息为 {res.Code}:{res.Message}");
            }
        }
        //临时消息发送都会发生这个异常, 除了魔改或等go-cqhttp更新没办法
        catch (CqApiCallFailedException) { }
    }
}

public class BindStateSession : ISession
{

    public SessionId SessionId { get; set; }

    public IEnumerator<EventWaiter> Enumerator { get; set; } = null!;
}