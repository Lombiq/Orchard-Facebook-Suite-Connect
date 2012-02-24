using Orchard.Events;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.EventHandlers
{
    public interface IFacebookConnectEventHandler : IEventHandler
    {
        void UserUpdated(IFacebookUser facebookUser);
    }
}
