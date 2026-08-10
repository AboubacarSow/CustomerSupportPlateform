using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace webAdmin.Utilities;

public class TokenContainer : INotifyPropertyChanged
{

    
    private string? _token;

    public string AccessToken
    {
        get => _token!;
        set
        {
            _token = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int ExpiresIn { get;private set; }
    public string? RefreshToken { get;private set; }

    public void SetExpiresIn(int expiresIn) => ExpiresIn = expiresIn;
    public void SetRefreshToken(string refreshToken) => RefreshToken = refreshToken;
    public void SetAccessToken(string accessToken) => AccessToken = accessToken;

    public string GetAccessToken() => AccessToken?? null!;

    public void Clear()
    {
        AccessToken = string.Empty;
        RefreshToken= string.Empty;
        ExpiresIn= 0;
    }


}

public class TokenHandler : DelegatingHandler
{
    private readonly TokenContainer _tokentContainer ;

    public TokenHandler(TokenContainer tokentContainer)
    {
        _tokentContainer = tokentContainer;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token=_tokentContainer.AccessToken;   
        if (!string.IsNullOrEmpty(token) || token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

