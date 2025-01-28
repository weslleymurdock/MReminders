namespace MReminders.Mobile.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AuthorizeAttribute(bool requiresAuthentication) : Attribute
{ 
    public bool RequiresAuthentication { get; set; } = requiresAuthentication;
}
