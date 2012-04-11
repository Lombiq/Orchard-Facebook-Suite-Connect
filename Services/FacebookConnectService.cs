using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement; // For generic ContentManager methods
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Piedone.Facebook.Suite.EventHandlers;
using Piedone.Facebook.Suite.Models;
using Facebook;
using Orchard.Mvc;

namespace Piedone.Facebook.Suite.Services
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectService : IFacebookConnectService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFacebookSuiteService _facebookSuiteService;
        private readonly IFacebookConnectEventHandler _eventHandler;

        #region Session handling
        private const string _sessionName = "Piedone.Facebook.Suite.Connect.Models.FacebookSession";
        private FacebookSession _session;
        public FacebookSession Session
        {
            get
            {
                if (_session == null)
                {
                    var session = _httpContextAccessor.Current().Session[_sessionName];
                    if (session != null)
                    {
                        _session = session as FacebookSession;
                    }
                }

                return _session;
            }

            private set
            {
                _httpContextAccessor.Current().Session[_sessionName] = _session = value;
            }
        }

        public void SetSession(long userId, string accessToken, DateTime expiresUtc)
        {
            Session = new FacebookSessionImpl(userId, accessToken, expiresUtc);
        }

        public void DestroySession()
        {
            Session = null;
        }

        private FacebookClient _clientForSession;
        public FacebookClient ClientForSession
        {
            get
            {
                if (_clientForSession == null && Session != null)
                {
                    _clientForSession = _facebookSuiteService.GetNewClient();
                    _clientForSession.AccessToken = Session.AccessToken;
                }

                return _clientForSession;
            }
        }
        #endregion

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public FacebookConnectService(
            IAuthenticationService authenticationService,
            IContentManager contentManager,
            IHttpContextAccessor httpContextAccessor,
            IFacebookSuiteService facebookSuiteService,
            IFacebookConnectEventHandler eventHandler)
        {
            _authenticationService = authenticationService;
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;
            _facebookSuiteService = facebookSuiteService;
            _eventHandler = eventHandler;

            Logger = NullLogger.Instance; // Constructor injection of ILogger fails
            T = NullLocalizer.Instance;
        }


        public IFacebookUser FetchMe()
        {
            if (!this.IsAuthenticated()) return null;

            try
            {
                dynamic me = ClientForSession.Get("me");

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
            catch (FacebookApiException ex)
            {
                throw new OrchardException(T("Error in retrieving Facebook user data: {0}", ex.Message), ex); // Useless to localize
            }
        }

        public bool UserIsValid(IFacebookUser facebookUser, IFacebookConnectSettings settings, out IEnumerable<FacebookConnectValidationKey> errors)
        {
            var errorsList = new List<FacebookConnectValidationKey>();


            if (!this.IsAuthenticated()) errorsList.Add(FacebookConnectValidationKey.NotAuthenticated);
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
            if (facebookId == 0) return null;

            return _contentManager
                .Query<FacebookUserPart, FacebookUserPartRecord>()
                .Where(u => u.FacebookUserId == facebookId).List().FirstOrDefault<FacebookUserPart>();
        }


        private class FacebookSessionImpl : FacebookSession
        {
            public FacebookSessionImpl(long userId, string accessToken, DateTime expiresUtc)
            {
                UserId = userId;
                AccessToken = accessToken;
                ExpiresUtc = expiresUtc;
            }
        }
    }
}