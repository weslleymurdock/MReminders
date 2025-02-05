using System.Threading.Tasks;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Domain.Enums;

namespace MReminders.Mobile.Infrastructure.Storages;
public class TokenStorage(IProtectedStorage<Token> storage) : ITokenStorage
{
    private const string BasicTokenKey = "basic_token";
    private const string BearerTokenKey = "bearer_token";
    private const string BiometricTokenKey = "biometric_token";

    public async Task SaveTokenAsync(Token token)
    {
        if (token.Kind == TokenKind.Basic)
        {
            await storage.SetAsync(BasicTokenKey, token);
            return;
        }
        if (token.Kind == TokenKind.Bearer)
        {
            await storage.SetAsync(BearerTokenKey, token);
            return;
        }
        if (token.Kind == TokenKind.Biometric)
        {
            await storage.SetAsync(BiometricTokenKey, token);
            return;
        }
    }


    public async Task<Token> GetTokenAsync(TokenKind kind) => kind == TokenKind.Bearer ?
        await storage.GetAsync(BearerTokenKey) :
        kind == TokenKind.Basic ?
        await storage.GetAsync(BasicTokenKey) :
        await storage.GetAsync(BiometricTokenKey);

}
