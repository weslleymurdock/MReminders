namespace MReminders.API.Application.Responses.Account;

public class UserResponse
{
    public string Id { get; set; }= string.Empty;    
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty ;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = [];
}