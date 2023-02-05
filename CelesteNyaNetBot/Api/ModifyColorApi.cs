using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class ModifyColorApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/modifyColor", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(EmptyNayResponseData);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; }

    [JsonPropertyName("color")]
    public string? Color { get; }

    public ModifyColorApi(string session, string qqNumber, string? color) : base(session)
    {
        QQNumber = qqNumber;
        Color = color;
    }
}
