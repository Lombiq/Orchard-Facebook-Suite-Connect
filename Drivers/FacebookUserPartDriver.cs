using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Drivers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPartDriver : ContentPartDriver<FacebookUserPart>
    {
        protected override DriverResult Display(FacebookUserPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FacebookUser",
                () => shapeHelper.Parts_FacebookUser());
        }
    }
}