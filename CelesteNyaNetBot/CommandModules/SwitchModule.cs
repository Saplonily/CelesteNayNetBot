using SaladimQBot.Core;
using System.Text;
using SaladimQBot.Extensions;
using SaladimQBot.GoCqHttp;
using SaladimQBot.Shared;
using Microsoft.Extensions.DependencyInjection;
using CelesteNyaNetBot.Services;
using SixLabors.ImageSharp.PixelFormats;

namespace CelesteNyaNetBot.CommandModules;

public class SwitchModule : CommandModule
{
    protected INyaService nyaService;
    protected IServiceProvider service;

    public SwitchModule(INyaService nyaService, IServiceProvider service)
    {
        this.nyaService = nyaService;
        this.service = service;
    }

    [Command("change_name", true)]
    public void ModifyName(string newName)
    {
        long? groupIdWay = null;
        if (Content.Message is IGroupMessage groupMsg)
            groupIdWay = groupMsg.Group.GroupId;
        else if (Content.Message is IPrivateMessage privateMessage)
            groupIdWay = privateMessage.TempSourceGroupId;

        if (!LoginModule.NameRegex.IsMatch(newName))
        {
            Content.Message.Sender.SendMessageAsync(groupIdWay, TipStrings.TipInvalidName);
            return;
        }
        var (res, data) = nyaService.ModifyNameAsync(Content.ExecutorId, newName).GetResultOfAwaiter();
        if (res is null || data is null)
        {
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipInternalError);
            return;
        }
        try
        {
            if (res.Code is 200)
            {
                if (data.Cooldown == null)
                {
                    Content.Executor.SendMessageAsync(groupIdWay, string.Format(TipStrings.TipNameChanged, newName));
                    if (Content.Message is IGroupMessage groupMsg2)
                        groupMsg2.Sender.SetGroupCardAsync(newName);
                }
                else
                {
                    string msg = string.Format(
TipStrings.TipNameChangeCooldowning,
                        TimeSpan.FromSeconds(data.Cooldown.Value).ToString(TipStrings.TipNameChangeCooldowningTimeSpanFormat)
                        );
                    Content.Executor.SendMessageAsync(groupIdWay, msg);
                }
            }
            else if (res.Code is 201)
            {
                Content.Executor.SendMessageAsync(groupIdWay, TipStrings.TipAccountNotBinded);
            }
            else
            {
                Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipInternalErrorWithCode, res.Code, res.Message));
            }
        }
        catch (CqApiCallFailedException) { }
    }

    [Command("change_prefix")]
    public void ModifyPrefix(string prefix)
    {
        var res = nyaService.ModifyPrefixAsync(Content.ExecutorId, prefix).GetResultOfAwaiter();
        if (res is null)
        {
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipInternalError);
            return;
        }
        if (res.Code == 200)
        {
            Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipPrefixChanged, prefix));
        }
        else if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerPrefixNotExists))
            {
                Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipNotOwnedPrefix, prefix));
            }
            else if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
            {
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
            }
        }
    }

    [Command("change_prefix")]
    public void ModifyPrefix()
    {
        var res = nyaService.ModifyPrefixAsync(Content.ExecutorId, null).GetResultOfAwaiter();
        if (res is null)
        {
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipInternalError);
            return;
        }
        if (res.Code == 200)
        {
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipPrefixReset);
        }
        else if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
            {
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
            }
        }
    }

    [Command("get_prefixs")]
    public void GetPrefixs()
    {
        var (res, data) = nyaService.GetPrefixsAsync(Content.ExecutorId).GetResultOfAwaiter();
        if (res is null || data is null) return;
        if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
            {
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
            }
        }
        else if (res.Code == 200)
        {
            if (data.Prefixs.Count == 0)
            {
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipNotOwnedAnyPrefix);
                return;
            }
            string resultString = TipStrings.TipPrefixListingHead
                + string.Join('\n', data.Prefixs.Select(o => o.Prefix));
            Content.MessageWindow.SendTextMessageAsync(resultString);
        }
    }

    [Command("get_prefixes")]
    public void GetPrefixes() => GetPrefixs();

    [Command("get_colors")]
    public void GetColors()
    {
        var (res, data) = nyaService.GetColorsAsync(Content.ExecutorId).GetResultOfAwaiter();
        if (res is null || data is null) return;
        if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
        }
        else if (res.Code == 200)
        {
            if (data.Colors.Count == 0)
            {
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipNotOwnedAnyColor);
            }
            else
            {
                var (ures, udata) = nyaService.GetUserNameAsync(Content.ExecutorId).GetResultOfAwaiter();
                if (ures is null || udata is null) return;
                if (ures.Code == 200)
                {
                    string prefixAndName = $"[{udata.Prefix}] - {udata.UserName}";
                    var ds = service.GetRequiredService<DrawingService>();

                    // linq不支持span, 别问为什么不用span
                    var colors = data.Colors
                        .Select(o => o.Color[1..])
                        .Select(s => new SixLabors.ImageSharp.Color(new Rgba32(
                            byte.Parse(s[0..2], System.Globalization.NumberStyles.HexNumber),
                            byte.Parse(s[2..4], System.Globalization.NumberStyles.HexNumber),
                            byte.Parse(s[4..6], System.Globalization.NumberStyles.HexNumber)
                            )));
                    Uri fUri = new(ds.Draw(prefixAndName, colors.ToArray()));
                    IMessageEntityBuilder b = Content.Client.CreateMessageBuilder();
                    b.WithImage(fUri);
                    b.WithText($"\n{TipStrings.TipColorListingTail}");
                    Content.MessageWindow.SendMessageAsync(b.Build());
                }
                else
                {
                    Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipInternalErrorWithCode, ures.Code, ures.Message));
                }
            }
        }
    }

    [Command("change_color")]
    public void ModifyColor(string color)
    {
        if (int.TryParse(color, out _)) return;
        var res = nyaService.ModifyColorAsync(Content.ExecutorId, color).GetResultOfAwaiter();
        if (res is null)
        {
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipInternalError);
        }
        else if (res.Code == 200)
        {
            Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipColorChanged, color));
        }
        else if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerColorNotExists))
                Content.MessageWindow.SendTextMessageAsync(string.Format(TipStrings.TipNotOwnedColor, color));
            else if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
        }
    }

    [Command("change_color")]
    public void ModifyColor()
    {
        var res = nyaService.ModifyColorAsync(Content.ExecutorId, null).GetResultOfAwaiter();
        if (res is null)
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipInternalError);
        else if (res.Code == 200)
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipColorReset);
        else if (res.Code == 201 && res.Message.Contains(TipStrings.MarkerAccountNotBinded))
            Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
    }

    [Command("change_color")]
    public void ModifyColor(int index)
    {
        var (res, data) = nyaService.GetColorsAsync(Content.ExecutorId).GetResultOfAwaiter();
        if (res is null || data is null) return;
        if (res.Code == 201)
        {
            if (res.Message.Contains(TipStrings.MarkerAccountNotBinded))
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipAccountNotBinded);
        }
        else if (res.Code == 200)
        {
            if (data.Colors.Count == 0)
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipNotOwnedAnyColor);
            else if (index is > 0 && index <= data.Colors.Count)
                ModifyColor(data.Colors[index - 1].Color);
            else
                Content.MessageWindow.SendTextMessageAsync(TipStrings.TipColorIndexOutOfRange);
        }
    }
}
