using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class GetColorsApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/getColors", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(GetColorsResponseData);

    [JsonPropertyName("qqnumber")]
    public string QqNumber { get; }

    public GetColorsApi(string session, string qqNumber) : base(session)
    {
        QqNumber = qqNumber;
    }
}
