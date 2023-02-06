using CelesteNyaNetBot.Response;

namespace CelesteNyaNetBot;

public interface INyaService
{
    Task<(NyaResponse?, CreateAccountResponseData?)> CreateAccountAsync(long userId, string userName);
    Task<NyaResponse?> DeleteAccountAsync(long userId);
    Task<(NyaResponse?, GetUserNameResponseData?)> GetUserNameAsync(long userId);
    Task<(NyaResponse?, RequestAuthResponseData?)> RequestAuthAsync(long userId);
    Task<(NyaResponse?, ModifyNameResponseData?)> ModifyNameAsync(long userId, string newName);
    Task<NyaResponse?> ModifyPrefixAsync(long userId, string? prefix);
    Task<(NyaResponse?, GetPrefixsResponseData?)> GetPrefixsAsync(long userId);
    Task<(NyaResponse?, GetColorsResponseData?)> GetColorsAsync(long userId);
    Task<NyaResponse?> ModifyColorAsync(long userId, string? color);
}
