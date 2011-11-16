using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Handlers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPartHandler : ContentHandler
    {
        public FacebookUserPartHandler(IRepository<FacebookUserPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<FacebookUserPart>("User"));
            // Enable the FacebookUser content type so we can use the content manager to manage FacebookUserParts
            //Filters.Add(new ActivatingFilter<FacebookUserPart>("FacebookUser"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}