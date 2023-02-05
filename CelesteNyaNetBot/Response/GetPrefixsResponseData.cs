using System.Collections;
using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class GetPrefixsResponseData : NyaResponseData
{
    public record PrefixObject([property: JsonPropertyName("prefix")] string Prefix);

    [JsonPropertyName("prefixs")]
    public List<PrefixObject> Prefixs { get; set; } = null!;
}
