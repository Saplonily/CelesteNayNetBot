using System.Collections;
using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Response;

public class GetColorsResponseData : NyaResponseData
{
    public record ColorObject([property: JsonPropertyName("color")] string Color);

    [JsonPropertyName("colors")]
    public List<ColorObject> Colors { get; set; } = null!;
}
