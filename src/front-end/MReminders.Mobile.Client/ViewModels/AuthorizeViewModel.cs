using MReminders.Mobile.Client.Views;
using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(true)]
public partial class AuthorizeViewModel : BaseViewModel
{
    #region Fields

    #endregion
    
    public AuthorizeViewModel()
    {
        var type = this.GetType();

        if (Attribute.GetCustomAttribute(type, typeof(AuthorizeAttribute)) is not AuthorizeAttribute attribute || !attribute.RequiresAuthentication)
        {
            return;
        }
        ValidateToken().ConfigureAwait(false);
    }
     
    protected async Task ValidateToken()
    {
        var token = await tokenStorage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
        if (string.IsNullOrEmpty(token.Value) || token.Expires <= DateTime.UtcNow)
        {
            this.CurrentApplication!.MainPage = new LoginPage();
        }
    } 
}
