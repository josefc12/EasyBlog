using Blazored.LocalStorage;

namespace EasyBlog.Client.Services;

public class LoginStateService
{

    public event Action OnChange;
    private readonly ILocalStorageService _localStorage;
    private bool _isLoggedIn;
    private bool _isAdmin;

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            if (_isLoggedIn != value)
            {
                _isLoggedIn = value;
                NotifyStateChanged();
            }
        }
    }

    public bool IsAdmin
    {
        get => _isAdmin;
        set
        {
            if (_isAdmin != value)
            {
                _isAdmin = value;
                NotifyStateChanged();
            }
        }
    }

    public LoginStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        CheckLoginState().ConfigureAwait(false);
        CheckRole().ConfigureAwait(false);
    }

    private async Task CheckLoginState()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        IsLoggedIn = !string.IsNullOrWhiteSpace(token);
    }

    private async Task CheckRole()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            // Decode the token to extract the nickname
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            const string roleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            string role = jwtToken?.Claims.FirstOrDefault(c => c.Type == roleClaimType)?.Value ?? "";
            if (role == "Admin")
            {
                IsAdmin = true;
                return;
            }
            IsAdmin = false;
        }
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}