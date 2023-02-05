using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public interface ITokenService
{
    Task<(NyaResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName);
    Task<NyaResponse?> DeleteAccountAsync(long userId);
    Task<(NyaResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId);
    Task<(NyaResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId);
    Task<(NyaResponse?, ModifyNameResponseData?)> ModifyNameAsync(long userId, string newName);
}
