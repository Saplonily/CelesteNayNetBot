using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using SaladimQBot.Core;
using SaladimQBot.Extensions;

namespace CelesteNyaNetBot.CommandModules;

public class AdminModule : CommandModule
{
    protected LoggerService loggerService;
    protected List<long> adminIds;

    public AdminModule(IConfiguration config, LoggerService loggerService)
    {
        adminIds = config.GetRequiredSection("Bot:OpCmdWhitelist").Get<List<long>>() ?? new();
        this.loggerService = loggerService;
    }

    public override bool PreCheck(CommandContent content)
        => adminIds.Any(i => content.Message.Sender.UserId == i);

    [Command("update_announce", isMergeExcess: true)]
    public void UpdateAnnouncement(string msg)
    {
        try
        {
            const string FileName = "Saves.json";
            if (File.Exists(FileName))
            {
                JsonObject? jsonObject = JsonNode.Parse(File.ReadAllText(FileName)) as JsonObject;
                if (jsonObject is not null)
                {
                    jsonObject["announce"] = msg;
                    File.WriteAllText(FileName, jsonObject.ToJsonString());
                    Content.MessageWindow.SendTextMessageAsync("公告更新成功.");
                    return;
                }
            }

            File.WriteAllText(FileName, new JsonObject() { ["announce"] = msg }.ToJsonString());
            Content.MessageWindow.SendTextMessageAsync("公告创建成功.");
        }
        catch (Exception e)
        {
            Content.MessageWindow.SendTextMessageAsync($"异常发生:\n{e.Message}");
            loggerService.Logger.LogError(CelesteNyaNetBot.NyaBot, e, prefix: "error when try to update/create announce");
        }
    }
}
