using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public abstract class NyaResponseData
{
    [JsonIgnore]
    public NyaResponse Response { get; set; } = null!;
}
