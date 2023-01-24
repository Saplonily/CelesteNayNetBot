using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public class TestTokenService : ITokenService
{
    public Task<(NayResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName)
    {
        CreateAccountResponseData data = new()
        {
            Token = "114514TESTtoken",
            UserName = "your....name!!!"
        };
        var res = new NayResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<NayResponse?> DeleteAccountAsync(long userId)
    {
        var res = new NayResponse() { Code = 200 };
        return Task.FromResult(res)!;
    }

    public Task<(NayResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId)
    {
        GetUserNameResponseData data = new()
        {
            Token = "114514TESTtoken username",
            UserName = $"n_{userId}"
        };
        var res = new NayResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<(NayResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId)
    {
        RequestAuthResponseData data = new()
        {
            Token = "114514TESTtoken relogined"
        };
        var res = new NayResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }
}
