using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public class TestService : INyaService
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

    public Task<NyaResponse?> ModifyPrefixAsync(long userId, string? prefix)
    {
        var res = new NyaResponse() { Code = 200, Message = "user not" };
        return Task.FromResult(res)!;
    }

    public Task<(NyaResponse?, GetPrefixsResponseData?)> GetPrefixsAsync(long userId)
    {
        GetPrefixsResponseData data = new()
        {
            Prefixs = new()
            {
                new("celeste-5"),
                new("o2")
            }
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<(NyaResponse?, GetColorsResponseData?)> GetColorsAsync(long userId)
    {
        GetColorsResponseData data = new()
        {
            Colors = new()
            {
                new("#114514"),
                new("#223366")
            }
        };
        var res = new NyaResponse() { ResponseData = data, Code = 200 };
        return Task.FromResult((res, data))!;
    }

    public Task<NyaResponse?> ModifyColorAsync(long userId, string? color)
    {
        var res = new NyaResponse() { Code = 201, Message = "color not" };
        return Task.FromResult(res)!;
    }
}
