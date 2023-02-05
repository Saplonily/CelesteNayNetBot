using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class GetPrefixsApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/getPrefixs", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(GetPrefixsResponseData);

    [JsonPropertyName("qqnumber")]
    public string QqNumber { get; }

    public GetPrefixsApi(string session, string qqNumber) : base(session)
    {
        QqNumber = qqNumber;
    }
}
