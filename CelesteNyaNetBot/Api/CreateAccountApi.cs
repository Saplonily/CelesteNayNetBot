using System.Text.Json.Serialization;
using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot.Api;

public class CreateAccountApi : NayApi
{
    [JsonIgnore]
    public override Uri Uri => new("/bot/createAccount", UriKind.Relative);

    [JsonIgnore]
    public override Type ResponseType => typeof(CreateAccountResponseData);

    [JsonPropertyName("bindqq")]
    public string BindQQ { get; set; }

    [JsonPropertyName("username")]
    public string UserName { get; set; }

    public CreateAccountApi(string session, string bindQQ, string userName) : base(session)
    {
        BindQQ = bindQQ;
        UserName = userName;
    }
}
