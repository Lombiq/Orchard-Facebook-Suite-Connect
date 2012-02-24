using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Piedone.Facebook.Suite.Models;
using Orchard.ContentManagement;

namespace Piedone.Facebook.Suite.Handlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectWidgetPartHandler : ContentHandler
    {
        public FacebookConnectWidgetPartHandler(ISiteService siteService)
        {
            OnLoaded<FacebookConnectWidgetPart>((context, part) =>
            {
                part.PermissionsField.Loader(() => siteService.GetSiteSettings().As<FacebookConnectSettingsPart>().Permissions);
            });
        }
    }
}