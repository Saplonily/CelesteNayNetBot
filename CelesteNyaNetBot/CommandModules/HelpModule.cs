using SaladimQBot.Core;
using SaladimQBot.Extensions;

namespace CelesteNyaNetBot.CommandModules;

public class HelpModule : CommandModule
{
    [Command("help")]
    public void Help()
    {
        Content.MessageWindow.SendTextMessageAsync(TipStrings.TipHelp);
    }
}
