
namespace MReminders.API.Application.Responses.Account;

public class LoginResponse
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public IList<string> Roles { get; set; } = [];
} 
