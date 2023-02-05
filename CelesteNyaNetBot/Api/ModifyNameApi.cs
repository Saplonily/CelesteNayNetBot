using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class ModifyNameApi : NyaApi
{
    [JsonIgnore]
    public override Uri Uri => new("bot/modifyName", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(ModifyNameResponseData);

    [JsonPropertyName("qqnumber")]
    public string QQNumber { get; }

    [JsonPropertyName("username")]
    public string NewUserName { get; }

    public ModifyNameApi(string session, long qqNumber, string newUserName)
        : base(session)
    {
        QQNumber = qqNumber.ToString();
        NewUserName = newUserName;
    }
}
