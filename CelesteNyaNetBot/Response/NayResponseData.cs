using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public abstract class NayResponseData
{
    [JsonIgnore]
    public NayResponse Response { get; set; } = null!;
}
