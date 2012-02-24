using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Facebook.Web;
using Orchard;
using Orchard.ContentManagement; // For generic ContentManager methods
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Settings;
using Piedone.Avatars.Services;
using Piedone.Facebook.Suite.Models;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;
using Piedone.HelpfulLibraries.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;
using Piedone.Facebook.Suite.EventHandlers;

namespace Piedone.Facebook.Suite.Services
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectService : IFacebookConnectService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IContentManager _contentManager;
        private readonly IFacebookSuiteService _facebookSuiteService;
        private readonly IFacebookConnectEventHandler _eventHandler;
        private readonly Lazy<FacebookWebClient> _facebookWebClient;

        public long AuthenticatedFacebookUserId
        {
            get { return _facebookSuiteService.FacebookWebContext.UserId; }
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public FacebookConnectService(
            IAuthenticationService authenticationService,
            IContentManager contentManager,
            IFacebookSuiteService facebookSuiteService,
            IFacebookConnectEventHandler eventHandler)
        {
            _authenticationService = authenticationService;
            _contentManager = contentManager;
            _facebookSuiteService = facebookSuiteService;
            _eventHandler = eventHandler;
            _facebookWebClient = new Lazy<FacebookWebClient>(() => new FacebookWebClient(_facebookSuiteService.FacebookWebContext));

            Logger = NullLogger.Instance; // Constructor injection of ILogger fails
            T = NullLocalizer.Instance;
        }


        /// <inheritdoc/>
        /// <summary>
        /// WARNING: does not check if the user is authenticated with Orchard.
        /// </summary>
        /// <returns>True if the user is logged in on Facebook and connected to the app, false otherwise.</returns>
        public bool IsAuthenticated()
        {
            return _facebookSuiteService.FacebookWebContext.IsAuthenticated();
        }

        /// <inheritdoc/>
        public bool IsAuthorized(string[] permissions = null)
        {
            return _facebookSuiteService.FacebookWebContext.IsAuthorized(permissions);
        }

        // Esetleg bele egy IContent-be, és akkor az Update egyszerűbb?
        public IFacebookUser FetchMe()
        {
            if (!IsAuthenticated()) return null;

            try
            {
                dynamic me = _facebookWebClient.Value.Get("me");

                var user = _contentManager.New<FacebookUserPart>("User");

                user.FacebookUserId = long.Parse(me.id);
                user.Name = me.name;
                user.FirstName = me.first_name;
                user.LastName = me.last_name;
                //user.Email =  me.email ?? "";
                user.Link = me.link;
                user.FacebookUserName = me.username ?? "";
                user.Gender = me.gender;
                user.TimeZone = (int)me.timezone;
                user.Locale = ((string)me.locale).Replace('_', '-'); // Making locale Orchard-compatible
                user.IsVerified = (me.verified != null) ? me.verified : false; // Maybe it is possible that verified is set, but is false -> don't take automatically as true if it's set

                return user;
            }
            catch (Exception ex)
            {
                throw new OrchardException(T("Error in retrieving Facebook user data: " + ex.Message), ex); // Useless to localize
            }
        }

        public bool UserIsValid(IFacebookUser facebookUser, IFacebookConnectSettings settings, out IEnumerable<FacebookConnectValidationKey> errors)
        {
            var errorsList = new List<FacebookConnectValidationKey>();
            

            if (!String.IsNullOrEmpty(settings.Permissions) && !this.IsAuthorized(settings.Permissions)) errorsList.Add(FacebookConnectValidationKey.NoPermissionsGranted);
            if (settings.OnlyAllowVerified && !facebookUser.IsVerified) errorsList.Add(FacebookConnectValidationKey.NotVerified);

            errors = errorsList;

            return errorsList.Count == 0;
        }

        public void UpdateFacebookUser(IUser user, IFacebookUser facebookUser)
        {
            var part = user.As<FacebookUserPart>();

            // Could this be better, e.g. with Automapper?
            part.FacebookUserId = facebookUser.FacebookUserId;
            part.FacebookUserName = facebookUser.FacebookUserName;
            part.FirstName = facebookUser.FirstName;
            part.Gender = facebookUser.Gender;
            part.IsVerified = facebookUser.IsVerified;
            part.LastName = facebookUser.LastName;
            part.Link = facebookUser.Link;
            part.Locale = facebookUser.Locale;
            part.Name = facebookUser.Name;
            part.TimeZone = facebookUser.TimeZone;

            _eventHandler.UserUpdated(user.As<IFacebookUser>());
        }

        public IFacebookUser GetFacebookUser(long facebookId)
        {
            return _contentManager
                .Query<FacebookUserPart, FacebookUserPartRecord>()
                .Where(u => u.FacebookUserId == facebookId).List().FirstOrDefault<FacebookUserPart>();
        }
    }
}