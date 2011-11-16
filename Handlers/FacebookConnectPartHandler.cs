using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Handlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectPartHandler : ContentHandler
    {
        public FacebookConnectPartHandler(IRepository<FacebookConnectPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}