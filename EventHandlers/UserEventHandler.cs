using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Users.Events;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;

namespace Piedone.Facebook.Suite.EventHandlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class UserEventHandler : IUserEventHandler
    {
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly ISiteService _siteService;

        public UserEventHandler(
            IFacebookConnectService facebookConnectService,
            ISiteService siteService)
        {
            _facebookConnectService = facebookConnectService;
            _siteService = siteService;
        }

        public void Creating(UserContext context)
        {
        }

        public void Created(UserContext context)
        {
            if (!_facebookConnectService.IsAuthenticated()) return;

            // This can happen if someone connects to the site with FB, logs out then registers a new user while being logged in to FB.
            // This prevents to attach the same FB user to different local users.
            if (_facebookConnectService.AuthenticatedFacebookUserIsSaved()) return;

            var facebookUser = _facebookConnectService.FetchMe();
            IEnumerable<FacebookConnectValidationKey> errors;
            if (_facebookConnectService.UserIsValid(facebookUser, _siteService.GetSiteSettings().As<FacebookConnectSettingsPart>(), out errors))
            {
                _facebookConnectService.UpdateFacebookUser(context.User.As<IUser>(), facebookUser);
            }
        }

        public void LoggedIn(IUser user)
        {
            // We could update FB profile data also here so that it's up-to-date even if the user simply logs in with her/his local profile
            // that's attached to a FB profile. This doesn't seem necessary and would mean that if the user logs in with the Connect button,
            // profile data would be updated twice.
        }

        public void LoggedOut(IUser user)
        {
        }

        public void AccessDenied(IUser user)
        {
        }

        public void ChangedPassword(IUser user)
        {
        }

        public void SentChallengeEmail(IUser user)
        {
        }

        public void ConfirmedEmail(IUser user)
        {
        }

        public void Approved(IUser user)
        {
        }
    }
}