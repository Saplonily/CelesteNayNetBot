using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class GetUserNameApi : NyaApi
{
    [JsonIgnore]
    public override Type ResponseType => typeof(GetUserNameResponseData);

    [JsonIgnore]
    public override Uri Uri => new("bot/getUsername", UriKind.Relative);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; set; }

    public GetUserNameApi(string session, string qqNumber) : base(session)
    {
        QQNumber = qqNumber;
    }
}
