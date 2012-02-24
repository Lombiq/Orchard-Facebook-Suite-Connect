
namespace Piedone.Facebook.Suite.Models
{
    public interface IFacebookConnectSettings
    {
        bool AutoLogin { get; }
        bool OnlyAllowVerified { get; }
        string Permissions { get; }
        bool SimpleRegistration { get; }
    }
}
