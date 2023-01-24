using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Api;

public abstract class NayApi
{
    [JsonIgnore]
    public abstract Uri Uri { get; }

    [JsonPropertyName("session")]
    public string Session { get; set; }

    [JsonIgnore]
    public abstract Type ResponseType { get; }

    protected NayApi(string session)
    {
        Session = session;
    }
}
