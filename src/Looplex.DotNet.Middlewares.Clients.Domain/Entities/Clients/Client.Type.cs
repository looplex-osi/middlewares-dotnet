using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.Clients.Domain.Entities.Clients;

public partial class Client 
{
    private string _displayName = null!;
    private DateTimeOffset _expirationTime;
    private DateTimeOffset _notBefore;
    private string _secret = null!;
    
    [JsonProperty("displayName")]
    public required string DisplayName 
    {
        get => _displayName;
        set
        {
            if (value != _displayName)
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("expirationTime")]
    public DateTimeOffset ExpirationTime
    {
        get => _expirationTime;
        set
        {
            if (value != _expirationTime)
            {
                _expirationTime = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("notBefore")]
    public DateTimeOffset NotBefore
    {
        get => _notBefore;
        set
        {
            if (value != _notBefore)
            {
                _notBefore = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("secret")]
    public required string Secret
    {
        get => _secret;
        set
        {
            if (value != _secret)
            {
                _secret = value;
                OnPropertyChanged();
            }
        }
    }
}