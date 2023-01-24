using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class RequestAuthResponseData : NayResponseData
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
}
