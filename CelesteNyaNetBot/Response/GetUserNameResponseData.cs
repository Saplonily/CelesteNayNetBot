using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class GetUserNameResponseData : NayResponseData
{
    [JsonPropertyName("uid")]
    public int Uid { get; set; }

    [JsonPropertyName("bind_number")]
    public long BindNumber { get; set; }

    [JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = null!;

    [JsonPropertyName("color")]
    public string Color { get; set; } = null!;

    [JsonPropertyName("gametime")]
    public long GameTime { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("gold")]
    public int Gold { get; set; }

    [JsonPropertyName("lastsigntime")]
    public long? LastSignTime { get; set; }

    [JsonPropertyName("lastlogintime")]
    public long? LastLoginTime { get; set; }

    [JsonPropertyName("lastlogoutime")]
    public long? LastLogoutTime { get; set; }
}
