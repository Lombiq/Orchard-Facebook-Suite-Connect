using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Handlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectSettingsPartHandler : ContentHandler
    {
        public FacebookConnectSettingsPartHandler(IRepository<FacebookConnectSettingsPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<FacebookConnectSettingsPart>("Site"));
        }
    }
}