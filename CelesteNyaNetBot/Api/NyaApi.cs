using System.Text.Json.Serialization;

namespace CelesteNyaNetBot.Api;

public abstract class NyaApi
{
    [JsonIgnore]
    public abstract Uri Uri { get; }

    [JsonPropertyName("session")]
    public string Session { get; set; }

    [JsonIgnore]
    public abstract Type ResponseType { get; }

    protected NyaApi(string session)
    {
        Session = session;
    }
}
