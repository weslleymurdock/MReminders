using MReminders.Mobile.Domain.Enums;

namespace MReminders.Mobile.Domain.Entities;

public class Token : IEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Value { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsActive { get => DateTime.UtcNow > Expires; }
    public TokenKind Kind { get; set; }
}
