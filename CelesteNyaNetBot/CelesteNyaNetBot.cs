using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saladim.SalLogger;
using SaladimQBot.Core;
using SaladimQBot.Extensions;
using SaladimQBot.GoCqHttp;
using SaladimQBot.Shared;

namespace CelesteNyaNetBot;

public class CelesteNyaNetBot
{
    public const string NyaBot = "NyaBot";
    protected CqWebSocketClient wsClient;
    protected Logger logger;
    protected Pipeline<IClientEvent> eventPipeline;
    protected Pipeline<IMessage> messagePipeline;

    protected IServiceProvider serviceProvider;
    protected INyaService tokenService;
    protected MemorySessionService memorySessionService;
    protected CoroutineService coroutineService;
    protected SimCommandExecuter simCmd;

    public CelesteNyaNetBot(IConfiguration configuration, INyaService tokenService, IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<LoggerService>().Logger;
        coroutineService = serviceProvider.GetRequiredService<CoroutineService>();
        memorySessionService = serviceProvider.GetRequiredService<MemorySessionService>();
        simCmd = serviceProvider.GetRequiredService<SimCommandService>().Executor;
        this.tokenService = tokenService;
        this.serviceProvider = serviceProvider;

        wsClient = new(configuration["Connection:WebSocketUri"]!, LogLevel.Trace);
        wsClient.OnLog += s => logger.LogInfo(NyaBot, s);
        wsClient.OnClientEventOccurred += WsClient_OnClientEventOccured;
        wsClient.OnStoppedUnexpectedly += WsClient_OnStoppedUnexpectedly;

        eventPipeline = ConfigureEventPipeline(new());
        messagePipeline = ConfigureMessagePipeline(new());
    }

    private void WsClient_OnStoppedUnexpectedly(Exception obj)
    {
        Task.Run(async () =>
        {
            logger.LogError(NyaBot, obj, $"Client session stopped unexpectetedly! " +
                $"Next connection try will be started in 4s.");
            tryReConnect:
            await Task.Delay(4000).ConfigureAwait(false);
            try
            {
                await wsClient.StartAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(NyaBot, e, prefix: "Error when trying to reconnect.", suffix: "Next connection retry will be started in 4s.");
                goto tryReConnect;
            }
        });

    }

    public void WsClient_OnClientEventOccured(ClientEvent clientEvent)
    {
        Task.Run(ExecutePipeline).ContinueWith(t =>
        {
            if (t.Exception is not null)
                logger.LogError(NyaBot, t.Exception, "Error occured when trying to execute eventPipeline");
        });
        void ExecutePipeline()
        {
            eventPipeline.ExecuteAsync(clientEvent).ContinueWith(t =>
            {
                if (t.Exception is not null)
                {
                    logger.LogError(NyaBot, t.Exception);
                }
            });
        }
    }

    public Pipeline<IClientEvent> ConfigureEventPipeline(Pipeline<IClientEvent> pipeline)
    {
        //转发消息事件给消息处理管线
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IClientMessageReceivedEvent msgEvent)
            {
                await messagePipeline.ExecuteAsync(msgEvent.Message).ConfigureAwait(false);
            }
            await next().ConfigureAwait(false);
        });
        //监听群名片修改事件, 如果不是从正常名片改回其他名片就不动
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IClientGroupMemberCardChangedEvent cardChangedEvent)
            {
                var (_, data) = await tokenService.GetUserNameAsync(cardChangedEvent.User.UserId).ConfigureAwait(false);
                if (data is not null)
                {
                    string toName = data.UserName;
                    if (cardChangedEvent.To != toName)
                    {
                        await cardChangedEvent.User.SetGroupCardAsync(toName).ConfigureAwait(false);
                    }
                }
                else
                {
                    logger.LogWarn(NyaBot, "Group member card changed but error occurred when try getting the username.");
                }
            }
            await next().ConfigureAwait(false);
        });
        //退群注销
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IClientGroupMemberDecreasedEvent decreasedEvent)
            {
                await tokenService.DeleteAccountAsync(decreasedEvent.User.UserId);
            }
            await next().ConfigureAwait(false);
        });
        //好友通过
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IClientFriendAddRequestedEvent frEvent)
            {
                await frEvent.Request.ApproveAsync();
            }
            await next().ConfigureAwait(false);
        });
        //协程
        pipeline.AppendMiddleware(async (e, next) =>
        {
            coroutineService.PushCoroutines(e);
            await next().ConfigureAwait(false);
        });
        return pipeline;
    }

    public Pipeline<IMessage> ConfigureMessagePipeline(Pipeline<IMessage> pipeline)
    {
        //debug用, 暂时屏蔽其他群/私聊消息
#if DEBUG
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IGroupMessage groupMessage && groupMessage.Group.GroupId is not (860355679 or 943024085))
                return;
            if (e is IPrivateMessage privateMessage && privateMessage.Sender.UserId is not (2748166392 or 1804381179))
                return;
            await next().ConfigureAwait(false);
        });
#endif
        //日志
        pipeline.AppendMiddleware(async (e, next) =>
        {
            if (e is IGroupMessage groupMessage)
            {
                logger.LogInfo(NyaBot, $"群 " +
                    $"{groupMessage.Group.GroupId} 中 " +
                    $"{groupMessage.Sender.GetFullName()} 说: " +
                    $"{groupMessage.MessageEntity.RawString}"
                    );
            }
            if (e is IPrivateMessage privateMessage)
            {
                logger.LogInfo(NyaBot, $"{privateMessage.Sender.Nickname} 私聊你: {privateMessage.MessageEntity.RawString}");
            }
            await next().ConfigureAwait(false);
        });
        //指令
        pipeline.AppendMiddleware(async (e, next) =>
        {
            await simCmd.MatchAndExecuteAllAsync(e).ConfigureAwait(false);
            await next().ConfigureAwait(false);
        });
        return pipeline;
    }

    public async Task StartAsync()
    {

        await wsClient.StartAsync().ConfigureAwait(false);
        logger.LogInfo(NyaBot, $"Bot '{wsClient.Self.Nickname.Value}({wsClient.Self.UserId})' logined.");
    }

    public async Task StopAsync()
    {
        logger.LogInfo("NayNetBot", "Stopping bot service...");
        await wsClient.StopAsync().ConfigureAwait(false);
    }
}
