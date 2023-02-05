using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class ModifyNameResponseData : NyaResponseData
{
    [JsonPropertyName("cooldown")]
    public long? Cooldown { get; set; }
}
