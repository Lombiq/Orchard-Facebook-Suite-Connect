﻿using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.UI.Notify;
using Piedone.Facebook.Suite.Helpers;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;

namespace Piedone.Facebook.Suite.Controllers
{
    [HandleError]
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class ConnectController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ConnectController(
            IContentManager contentManager,
            IFacebookConnectService facebookConnectService,
            INotifier notifier)
        {
            _contentManager = contentManager;
            _facebookConnectService = facebookConnectService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        /// <summary>
        /// Used when connecting through button click (not autologin)
        /// </summary>
        public ActionResult Connect(int connectId, string returnUrl = "")
        {
            var settings = _contentManager.Get<FacebookConnectPart>(connectId);

            if (!_facebookConnectService.Authorize(
                            permissions: FacebookConnectHelper.PermissionSettingsToArray(settings.Permissions),
                            onlyAllowVerified: settings.OnlyAllowVerified))
            {
                var errorMessages = new Dictionary<FacebookConnectValidationKey, LocalizedString>(3);
                errorMessages[FacebookConnectValidationKey.NoPermissionsGranted] = T("You haven't granted the requested permissions. Therefore, we were unable to log you in.");
                errorMessages[FacebookConnectValidationKey.NotAuthenticated] = T("You're not logged in at Facebook.");
                errorMessages[FacebookConnectValidationKey.NotVerified] = T("You're not a verified Facebook user. Only verified users are allowed to register, so please verify your account.");


                foreach (var error in _facebookConnectService.ValidationDictionary.Errors)
                {
                    _notifier.Warning(errorMessages[error.Key]);
                }
            }

            return this.RedirectLocal(returnUrl); // "this" is necessary, as this is from an extension (Orchard.Mvc.Extensions.ControllerExtensions)
        }
    }
}
