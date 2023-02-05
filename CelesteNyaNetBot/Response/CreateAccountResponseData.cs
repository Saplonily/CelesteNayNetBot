using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class CreateAccountResponseData : NyaResponseData
{
    [JsonPropertyName("bind_number")]
    public string BindNumber { get; set; } = null!;

    [JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = null!;

    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
}
