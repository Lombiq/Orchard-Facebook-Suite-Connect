using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Handlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectWidgetPartHandler : ContentHandler
    {
        public FacebookConnectWidgetPartHandler(Work<ISiteService> siteServiceWork)
        {
            OnActivated<FacebookConnectWidgetPart>((context, part) =>
                {
                    part.PermissionsField.Loader(() => siteServiceWork.Value.GetSiteSettings().As<FacebookConnectSettingsPart>().Permissions);
                });
        }
    }
}