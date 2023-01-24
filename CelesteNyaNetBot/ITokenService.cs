using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public interface ITokenService
{
    Task<(NayResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName);
    Task<NayResponse?> DeleteAccountAsync(long userId);
    Task<(NayResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId);
    Task<(NayResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId);
}
