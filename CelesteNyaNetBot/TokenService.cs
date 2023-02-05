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

    public Task<(NyaResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName)
        => PackedCallApiAsync<CreateAccountResponseData>(new CreateAccountApi(session, userId.ToString(), userName));

    public Task<(NyaResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId)
        => PackedCallApiAsync<RequestAuthResponseData>(new RequestAuthApi(session, userId.ToString()));

    public Task<(NyaResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId)
        => PackedCallApiAsync<GetUserNameResponseData>(new GetUserNameApi(session, userId.ToString()));

    public Task<(NyaResponse?, ModifyNameResponseData?)> ModifyNameAsync(long userId, string newName)
        => PackedCallApiAsync<ModifyNameResponseData>(new ModifyNameApi(session, userId, newName));
    public async Task<NyaResponse?> DeleteAccountAsync(long userId)
        => (await PackedCallApiAsync<EmptyNayResponseData>(new DeleteAccountApi(session, userId.ToString())).ConfigureAwait(false)).Item1;

    public async Task<(NyaResponse?, T?)> PackedCallApiAsync<T>(NyaApi api)
        where T : NyaResponseData
    {
        var res = await CallApiAsync<T>(api, new Uri(BaseUri, new Uri(BaseUri, api.Uri)));
        return (res, res?.ResponseData as T);
    }

    protected async Task<NyaResponse?> CallApiAsync<TResponseDataType>(NyaApi api, Uri uri)
        where TResponseDataType : NyaResponseData
    {
        string json = JsonSerializer.Serialize(api, api.GetType());
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        StreamReader reader = new(response.Content.ReadAsStream());
        string resultString = reader.ReadToEnd();
        JsonDocument doc = JsonDocument.Parse(resultString);
        NyaResponse? r = JsonSerializer.Deserialize<NyaResponse>(doc);
        if (r is not null)
        {
            r.ResponseData = JsonSerializer.Deserialize<TResponseDataType>(doc.RootElement.GetProperty("data"));
        }
        return r;
    }

}
