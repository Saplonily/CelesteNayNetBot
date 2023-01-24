using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class NayResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonIgnore]
    public NayResponseData? ResponseData { get; set; }
}