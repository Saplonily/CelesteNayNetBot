using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class RequestAuthApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("/bot/requestAuth", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(RequestAuthResponseData);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; set; }

    public RequestAuthApi(string session, string qqnumber) : base(session)
    {
        QQNumber = qqnumber;
    }
}
