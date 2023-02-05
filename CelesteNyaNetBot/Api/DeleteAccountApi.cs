using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class DeleteAccountApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/deleteAccount", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(EmptyNayResponseData);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; set; }

    public DeleteAccountApi(string session, string qqNumber) : base(session)
    {
        QQNumber = qqNumber;
    }
}
