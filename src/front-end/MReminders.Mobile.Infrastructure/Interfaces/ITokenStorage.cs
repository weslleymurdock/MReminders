using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Domain.Enums; 

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface ITokenStorage
{
    Task SaveTokenAsync(Token token);
    Task<Token> GetTokenAsync(TokenKind kind);
}
