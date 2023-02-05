using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class NyaResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonIgnore]
    public NyaResponseData? ResponseData { get; set; }
}