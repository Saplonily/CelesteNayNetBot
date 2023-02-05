using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class ModifyPrefixApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/modifyPrefix", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(EmptyNayResponseData);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; }

    [JsonPropertyName("prefix")]
    public string? Prefix { get; }

    public ModifyPrefixApi(string session, string qqNumber, string? prefix) : base(session)
    {
        QQNumber = qqNumber;
        Prefix = prefix;
    }
}
