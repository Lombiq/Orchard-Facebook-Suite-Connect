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
using Piedone.Facebook.Suite.Helpers;
using Piedone.Facebook.Suite.Models;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;
using Piedone.HelpfulLibraries.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Piedone.Facebook.Suite.Services
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectService : IFacebookConnectService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IContentManager _contentManager;
        private readonly IFacebookSuiteService _facebookSuiteService;
        private readonly IAvatarsService _avatarsService;
        private readonly ITaskFactory _taskFactory;

        public IServiceValidationDictionary<FacebookConnectValidationKey> ValidationDictionary { get; private set; }
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public FacebookConnectService(
            IAuthenticationService authenticationService,
            IMembershipService membershipService,
            IContentManager contentManager,
            IServiceValidationDictionary<FacebookConnectValidationKey> validationDictionary,
            IFacebookSuiteService facebookSuiteService,
            IAvatarsService avatarsService,
            ITaskFactory taskFactory)
        {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _contentManager = contentManager;
            ValidationDictionary = validationDictionary;
            _facebookSuiteService = facebookSuiteService;
            _avatarsService = avatarsService;
            _taskFactory = taskFactory;

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

        /// <inheritdoc/>
        /// <seealso cref="FacebookConnectService.IsAuthorized()"/>
        /// <exception cref="Orchard.OrchardException">Thrown if Facebook API calls fail.</exception>
        public bool Authorize(string[] permissions = null, bool onlyAllowVerified = false)
        {
            // User is not authenticated on Facebook or hasn't connected to our app or hasn't granted the needed permissions.
            if (!IsAuthorized(permissions))
            {
                if (!IsAuthenticated()) ValidationDictionary.AddError(FacebookConnectValidationKey.NotAuthenticated, T("User is not authenticated on Facebook."));
                else ValidationDictionary.AddError(FacebookConnectValidationKey.NoPermissionsGranted, T("The requested permissions were not granted."));

                return false;
            }

            // Now we know that the user is authenticated on Facebook and connected to our app.

            FacebookUserPart facebookUserPart = GetFacebookUserPart(_facebookSuiteService.FacebookWebContext.UserId); // Check if we already have the user's data saved

            var facebookClient = new FacebookWebClient(_facebookSuiteService.FacebookWebContext);

            Action<IUser> forceSignIn =
                u =>
                {
                    _authenticationService.SignOut();
                    _authenticationService.SignIn(
                        u,
                        true);
                };

            dynamic me = "";

            // We have previously saved the user's data
            if (facebookUserPart != null)
            {
                UpdateAvatarAsync(facebookUserPart);

                facebookClient.GetCompleted +=
                    (sender, e) =>
                    {
                        if (e.Error == null)
                        {
                            me = e.GetResultData();

                            // It is not handled if the user's name changed
                            //bool nameHasChanged = fbUserPart.Name != me.name;

                            // Update saved user
                            facebookUserPart = FacebookUserDataMapper.MapToFacebookUserPart(
                                new FacebookUserDataMapper(me),
                                facebookUserPart);
                            _contentManager.Flush(); // Else the update would not commit.

                            //if (nameHasChanged)
                            //{
                            //    var userPart = _contentManager.Get<Orchard.Users.Models.UserPart>(fbUserPart.UserId);
                            //    userPart.UserName = fbUserPart.Name;
                            //    userPart.NormalizedUserName = fbUserPart.Name.ToLowerInvariant(); // This is taken from the implementation, but should really be in a method of MembershipService 
                            //}
                        }
                        else
                        {
                            string message = "Error in retrieving Facebook user data: " + e.Error.Message;
                            Logger.Error(e.Error, message);
                        }
                    };
                facebookClient.GetAsync("me"); // Updating user data can run in the background

                forceSignIn(facebookUserPart.As<Orchard.Users.Models.UserPart>().As<IUser>());
            }
            // We don't currently have the user's data
            else
            {
                try
                {
                    me = facebookClient.Get("me"); // First login should run synchronously to get user data
                    var dataMapper = new FacebookUserDataMapper(me);

                    if (onlyAllowVerified && !dataMapper.IsVerified)
                    {
                        ValidationDictionary.AddError(FacebookConnectValidationKey.NotVerified, T("User is not verified."));
                        return false;
                    }

                    // Does not need to verifiy user unicity as there can be more people with the same name.
                    // Can lead to confusion, must rethink.
                    var random = new Random();
                    var authenticatedUser = _membershipService.CreateUser(
                        new CreateUserParams(
                            dataMapper.Name,
                            random.Next().ToString(),
                            dataMapper.Email,
                            "",
                            "",
                            true));

                    facebookUserPart = authenticatedUser.As<FacebookUserPart>();
                    facebookUserPart = FacebookUserDataMapper.MapToFacebookUserPart(dataMapper, facebookUserPart);

                    UpdateAvatarAsync(facebookUserPart);

                    forceSignIn(authenticatedUser);
                }
                catch (Exception ex)
                {
                    string message = "Error in retrieving Facebook user data: " + ex.Message;
                    Logger.Error(ex, message);
                    throw new OrchardException(T(message), ex); // Useless to localize
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public FacebookUserPart GetFacebookUserPart(long facebookId)
        {
            return _contentManager
                .Query<FacebookUserPart, FacebookUserPartRecord>()
                .Where(u => u.FacebookUserId == facebookId).List().FirstOrDefault<FacebookUserPart>();
        }

        /// <inheritdoc/>
        public FacebookUserPart GetFacebookUserPart(int id)
        {
            var part = _contentManager
                .Query<FacebookUserPart, FacebookUserPartRecord>()
                .Where(u => u.Id == id).List().FirstOrDefault<FacebookUserPart>();

            // This happens if one is logged in at FB and in Orchard, but here not with FB creditentials.
            // Since FacebookUserPart is attached to the User type, an empty FacebookUserPart record will be created.
            if (part != null && part.FacebookUserId == 0) return null;
            return part;
        }

        /// <inheritdoc/>
        public FacebookUserPart GetAuthenticatedFacebookUserPart()
        {
            // _authenticationService.GetAuthenticatedUser().As<FacebookUserPart>(); would
            // return an empty object, under some circumstances, see GetFacebookUserPart(int id).
            var authenticatedUser = _authenticationService.GetAuthenticatedUser();
            if (authenticatedUser == null) return null;
            return GetFacebookUserPart(authenticatedUser.Id);
        }

        private void UpdateAvatarAsync(FacebookUserPart facebookUserPart)
        {
            if (String.IsNullOrEmpty(facebookUserPart.PictureLink)) return;

            using (var wc = new WebClient())
            {
                wc.DownloadDataCompleted += _taskFactory.BuildAsyncEventHandler<object, DownloadDataCompletedEventArgs>(
                    (sender, e) =>
                    {
                        if (e.Error == null)
                        {
                            var stream = new MemoryStream(e.Result);
                            _avatarsService.SaveAvatarFile(facebookUserPart.Id, stream, "jpg"); // We could look at the bytes to detect the file type, but rather not
                        }

                        else
                        {
                            string message = "Downloading of Facebok profile picture failed: " + e.Error.Message;
                            Logger.Error(e.Error, message);
                        }
                    }, false).Invoke;
                wc.DownloadDataAsync(new Uri(facebookUserPart.PictureLink));
            }
        }
    }
}