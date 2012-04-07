
namespace Piedone.Facebook.Suite.Models
{
    public interface IFacebookConnectSettings
    {
        bool OnlyAllowVerified { get; }
        string Permissions { get; }
        bool SimpleRegistration { get; }
    }
}
