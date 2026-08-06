using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace webAdmin.Utilities;

public record TokenContainer : INotifyPropertyChanged
{

    public TokenContainer(string accesstoken,string refreshToken,int expiresIn)
    {
        AccessToken = accesstoken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
    }
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
    public string RefreshToken { get;private set; }

    public void SetExpiresIn(int expiresIn) => ExpiresIn = expiresIn;
    public void SetRefreshToken(string refreshToken) => RefreshToken = refreshToken;
    public void SetAccessToken(string accessToken) => AccessToken = accessToken;
}

public class TokenHandler(TokenContainer tokentContainer) : DelegatingHandler
{
    private readonly TokenContainer _tokentContainer = tokentContainer;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token=_tokentContainer.AccessToken;   
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

