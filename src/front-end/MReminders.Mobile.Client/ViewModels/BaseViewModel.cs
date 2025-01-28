using CommunityToolkit.Mvvm.ComponentModel;
using MReminders.Mobile.Client.Views;
using MReminders.Mobile.Infrastructure.Attributes;
using MReminders.Mobile.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace MReminders.Mobile.Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region readonly services
    private readonly JwtSecurityTokenHandler handler = new();
    private readonly ITokenService _tokenService;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private string title = string.Empty;
    #endregion

    #region Fields
    private bool _isAuthenticated;
    #endregion
    
    public BaseViewModel()
    {
        var type = this.GetType();

        if (Attribute.GetCustomAttribute(type, typeof(AuthorizeAttribute)) is not AuthorizeAttribute attribute || !attribute.RequiresAuthentication)
        {
            return;
        }
        ValidateToken().ConfigureAwait(false);
    }

    private async Task ValidateToken()
    {
        var token = await SecureStorage.GetAsync("jwt-token");
        if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
        {
            Microsoft.Maui.Controls.Application.Current!.MainPage = new LoginPage();
        }
    }
    private bool IsTokenExpired(string token)
    {
        return handler.ReadToken(token) is not JwtSecurityToken jwtToken || jwtToken.ValidTo <= DateTime.UtcNow;
    }
}