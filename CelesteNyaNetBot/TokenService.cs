using System.Text;
using System.Text.Json;
using CelesteNyaNetBot.Api;
using CelesteNyaNetBot.Response;
using Microsoft.Extensions.Configuration;

namespace CelesteNyaNetBot;

public class TokenService : ITokenService
{
    protected string session;

    public Uri BaseUri { get; protected set; }


    protected HttpClient httpClient;

    public TokenService(IConfiguration configuration)
    {
        BaseUri = new Uri(configuration["TokenService:BaseUrl"]!);
        session = configuration["TokenService:Session"]!;
        httpClient = new();
    }

    public async Task<(NayResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName)
    {
        CreateAccountApi api = new(session, userId.ToString(), userName);
        var res = await CallApiAsync<CreateAccountResponseData>(api, new Uri(BaseUri, api.Uri));
        return (res, res?.ResponseData as CreateAccountResponseData);
    }

    public async Task<(NayResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId)
    {
        RequestAuthApi api = new(session, userId.ToString());
        var res = await CallApiAsync<RequestAuthResponseData>(api, new Uri(BaseUri, api.Uri));
        return (res, res?.ResponseData as RequestAuthResponseData);
    }

    public async Task<(NayResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId)
    {
        GetUserNameApi api = new(session, userId.ToString());
        var res = await CallApiAsync<GetUserNameResponseData>(api, new Uri(BaseUri, api.Uri));
        return (res, res?.ResponseData as GetUserNameResponseData);
    }

    public async Task<NayResponse?> DeleteAccountAsync(long userId)
    {
        DeleteAccountApi api = new(session, userId.ToString());
        var res = await CallApiAsync<EmptyNayResponseData>(api, new Uri(BaseUri, api.Uri));
        return res;
    }

    protected async Task<NayResponse?> CallApiAsync<TResponseDataType>(NayApi api, Uri uri)
        where TResponseDataType : NayResponseData
    {
        string json = JsonSerializer.Serialize(api, api.GetType());
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        StreamReader reader = new(response.Content.ReadAsStream());
        string resultString = reader.ReadToEnd();
        JsonDocument doc = JsonDocument.Parse(resultString);
        NayResponse? r = JsonSerializer.Deserialize<NayResponse>(doc);
        if (r is not null)
        {
            r.ResponseData = JsonSerializer.Deserialize<TResponseDataType>(doc.RootElement.GetProperty("data"));
        }
        return r;
    }
}
