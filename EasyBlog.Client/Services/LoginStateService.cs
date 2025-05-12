using Blazored.LocalStorage;

namespace EasyBlog.Client.Services;

public class LoginStateService
{

    public event Action OnChange;
    private readonly ILocalStorageService _localStorage;
    private bool _isLoggedIn;

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

    public LoginStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        CheckLoginState().ConfigureAwait(false);
    }

    private async Task CheckLoginState()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        IsLoggedIn = !string.IsNullOrWhiteSpace(token);
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}