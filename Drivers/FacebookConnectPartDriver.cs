using System;
using System.Dynamic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using Piedone.Avatars.Models;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;

namespace Piedone.Facebook.Suite.Drivers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectPartDriver : ContentPartDriver<FacebookConnectPart>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly IFacebookSuiteService _facebookSuiteService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        protected override string Prefix
        {
            get { return "Connect"; }
        }

        public FacebookConnectPartDriver(
            IAuthenticationService authenticationService,
            IFacebookConnectService facebookConnectService,
            IFacebookSuiteService facebookSuiteService,
            INotifier notifier
            )
        {
            _authenticationService = authenticationService;
            _facebookConnectService = facebookConnectService;
            _facebookSuiteService = facebookSuiteService;
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(FacebookConnectPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FacebookConnect",
                () =>
                {
                    var isAuthenticated = _authenticationService.IsAuthenticated();
                    var isConnected = _facebookConnectService.IsAuthorized(part.Permissions) && _facebookConnectService.AuthenticatedFacebookUserIsSaved();

                    IFacebookUser authenticatedFacebookUser = null;

                    if (!isAuthenticated && isConnected && part.AutoLogin)
                    {
                        authenticatedFacebookUser = _facebookConnectService.UpdateAuthenticatedFacebookUser();
                        _authenticationService.SignIn(authenticatedFacebookUser.As<IUser>(), false);
                        isAuthenticated = true;
                        //CurrentUser.PictureLink = !String.IsNullOrEmpty(avatar.ImageUrl) ? avatar.ImageUrl : currentFacebookUserPart.PictureLink;
                    }
                    else if (isConnected)
                    {
                        authenticatedFacebookUser = _facebookConnectService.GetAuthenticatedFacebookUser();
                    }

                    return shapeHelper.Parts_FacebookConnect(
                                IsAuthenticated: isAuthenticated,
                                IsConnected: isConnected,
                                AuthenticatedFacebookUser: authenticatedFacebookUser);
                });
        }

        // GET
        protected override DriverResult Editor(FacebookConnectPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_FacebookConnect_Edit",
                () =>
                {
                    if (!_facebookSuiteService.AppSettingsSet)
                    {
                        _notifier.Add(NotifyType.Error, T("Currently the Facebook app settings are not set, therefore Facebook Connect won't work properly. Please set the app settings first on the page Facebook Suite Settings under Settings."));
                    }

                    return shapeHelper.EditorTemplate(
                        TemplateName: "Parts/FacebookConnect",
                        Model: part,
                        Prefix: Prefix);
                });
        }

        // POST
        protected override DriverResult Editor(FacebookConnectPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}