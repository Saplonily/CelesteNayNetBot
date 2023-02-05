using System.Text.RegularExpressions;
using System.Xml.Linq;
using CelesteNyaNetBot.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    protected IServiceProvider serviceProvider;
    protected LoggerService loggerService;

    public LoginModule(
        ITokenService tokenService,
        MemorySessionService memorySessionService,
        CoroutineService coroutineService,
        IServiceProvider serviceProvider,
        LoggerService loggerService
        )
    {
        this.tokenService = tokenService;
        this.memorySessionService = memorySessionService;
        this.coroutineService = coroutineService;
        this.serviceProvider = serviceProvider;
        this.loggerService = loggerService;
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
            if (userNameStrs.Length == 0)
            {
                yield break;
            }
            if (userNameStrs.Length == 2)
            {
                if (userNameStrs[1] == "confirm")
                {
                    Content.Message.Sender.SendMessageAsync(groupIdWay, "请先使用 \"!bind 用户名\"指令后再使用该指令进行确认.");
                }
                yield break;
            }
            int timeout = int.Parse(serviceProvider.GetRequiredService<IConfiguration>()["Bot:BindTimeout"]!);
            if (!nameRegex.IsMatch(userName))
            {
                Content.Message.Sender.SendMessageAsync(groupIdWay, "昵称不符合规则！只允许大小写字母和数字以及符号_和^和.的组合.");
                yield break;
            }
            try
            {
                Content.Message.Sender.SendMessageAsync(groupIdWay,
                    $"你确定要将你的id绑定到用户名 \"{userName}\" 上吗, " +
                    $"使用!bind {userName} confirm确认该操作. " +
                    $"此操作将会在{timeout}秒后将失效. " +
                    $"使用!bind_cancel主动取消该操作."
                    );
            }
            //临时消息发送都会发生这个异常, 除了魔改或等go-cqhttp更新没办法
            catch (CqApiCallFailedException) { }

            var cmdConfirmWaiter = new CommandWaiter(
                Content.SimCommandExecuter,
                Content.Executor,
                "bind",
                new object[] { new string[] { userName, "confirm" } }
                );
            var cmdCancelWaiter = new CommandWaiter(
                Content.SimCommandExecuter,
                Content.Executor,
                "bind_cancel"
                );
            var tickWaiter = new TickWaiter(timeout);
            EventWaiter resultWaiter = null!;
            yield return new OrEventWaiter(w => resultWaiter = w, cmdConfirmWaiter, cmdCancelWaiter, tickWaiter);
            if (resultWaiter == tickWaiter)
            {
                Content.Executor.SendMessageAsync(groupIdWay, "确认超时, 请再次使用!bind指令进行绑定");
            }
            else if (resultWaiter == cmdCancelWaiter)
            {
                Content.Executor.SendMessageAsync(groupIdWay, "取消bind操作");
            }
            else if (resultWaiter == cmdConfirmWaiter)
            {
                var (res, data) = tokenService.CreateAccountAsync(Content.Executor.UserId, userName).Result;
                if (res is null)
                {
                    Content.Message.Sender.SendMessageAsync(groupIdWay, "内部异常");
                    yield break;
                }
                try
                {
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
                            groupMessage.Sender.SetGroupCardAsync(userName);
                    }
                    else if (res.Code is 201)
                    {
                        Content.Message.Sender.SendMessageAsync(groupIdWay, $"绑定失败, 已经存在绑定的账号。如需重新登陆请使用 !relogin");
                    }
                    else
                    {
                        Content.Message.Sender.SendMessageAsync(groupIdWay, $"内部异常，消息为 {res.Code}:{res.Message}");
                    }
                }
                catch (CqApiCallFailedException) { }
                yield break;
            }
        }
    }

    [Command("bind_cancel")]
    public void BindCancel() { }

    [Command("relogin")]
    public void Relogin()
    {
        var (res, data) = tokenService.RequestAuthAsync(Content.ExecutorId).Result;
        if (res is null)
        {
            Content.MessageWindow.SendTextMessageAsync("内部异常");
            return;
        }
        try
        {
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
            else
            {
                Content.MessageWindow.SendTextMessageAsync($"内部异常，消息为 {res.Code}, {res.Message}");
            }
        }
        //临时消息发送都会发生这个异常, 除了魔改或等go-cqhttp更新没办法
        catch (CqApiCallFailedException) { }
    }

    [Command("change_name", true)]
    public void ModifyName(string newName)
    {
        long? groupIdWay = null;
        if (Content.Message is IGroupMessage groupMsg)
        {
            groupIdWay = groupMsg.Group.GroupId;
        }
        else if (Content.Message is IPrivateMessage privateMessage)
        {
            groupIdWay = privateMessage.TempSourceGroupId;
        }
        if (!nameRegex.IsMatch(newName))
        {
            Content.Message.Sender.SendMessageAsync(groupIdWay, "昵称不符合规则！只允许大小写字母和数字以及符号_和^和.的组合.");
            return;
        }
        var (res, data) = tokenService.ModifyNameAsync(Content.ExecutorId, newName).GetResultOfAwaiter();
        if (res is null || data is null)
        {
            Content.MessageWindow.SendTextMessageAsync("内部异常");
            loggerService.Logger.LogError(CelesteNyaNetBot.NyaBot, "Error when trying call modify_name api, got null response entity.");
            return;
        }
        try
        {
            if (res.Code is 200)
            {
                if (data.Cooldown == null)
                {
                    Content.Executor.SendMessageAsync(groupIdWay, $"您的名称已更改为 \"{newName}\".");
                    if (Content.Message is IGroupMessage groupMsg2)
                        groupMsg2.Sender.SetGroupCardAsync(newName);
                }
                else
                {
                    Content.Executor.SendMessageAsync(groupIdWay, 
                        $"改名冷却中！ 剩余时间: {TimeSpan.FromSeconds(data.Cooldown.Value):d\\d\\ hh\\h\\:mm\\m\\:ss\\s}");
                }
            }
            else if (res.Code is 201)
            {
                Content.Executor.SendMessageAsync(groupIdWay, $"您还暂时未绑定您的群服账号, 请先使用\"!bind 用户名\"指令绑定账号");
            }
            else
            {
                Content.MessageWindow.SendTextMessageAsync($"内部异常，消息为 {res.Code}, {res.Message}");
            }
        }
        catch (CqApiCallFailedException) { }
    }
}

public class BindStateSession : ISession
{

    public SessionId SessionId { get; set; }

    public IEnumerator<EventWaiter> Enumerator { get; set; } = null!;
}