using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public class TestTokenService : ITokenService
{
    public Task<(NyaResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName)
    {
        CreateAccountResponseData data = new()
        {
            Token = "114514TESTtoken",
            UserName = "your....name!!!"
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<NyaResponse?> DeleteAccountAsync(long userId)
    {
        var res = new NyaResponse() { Code = 200 };
        return Task.FromResult(res)!;
    }

    public Task<(NyaResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId)
    {
        GetUserNameResponseData data = new()
        {
            Token = "114514TESTtoken username",
            UserName = $"n_{userId}"
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<(NyaResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId)
    {
        RequestAuthResponseData data = new()
        {
            Token = "114514TESTtoken relogined"
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<(NyaResponse?, ModifyNameResponseData?)> ModifyNameAsync(long userId, string newName)
    {
        ModifyNameResponseData data = new()
        {
            Cooldown = 2417479
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }
}
