using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class RequestAuthResponseData : NyaResponseData
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
}
