using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;

namespace Piedone.Facebook.Suite.Drivers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPartDriver : ContentPartDriver<FacebookUserPart>
    {
        protected override DriverResult Display(FacebookUserPart part, string displayType, dynamic shapeHelper)
        {
            // There is no FB profile saved
            if (part.FacebookUserId == 0) return null;

            return ContentShape("Parts_FacebookUser",
                () => shapeHelper.Parts_FacebookUser());
        }

        protected override void Exporting(FacebookUserPart part, ExportContentContext context)
        {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("FacebookUserId", part.FacebookUserId);
            element.SetAttributeValue("Name", part.Name);
            element.SetAttributeValue("FirstName", part.FirstName);
            element.SetAttributeValue("LastName", part.LastName);
            element.SetAttributeValue("Link", part.Link);
            element.SetAttributeValue("FacebookUserName", part.FacebookUserName);
            element.SetAttributeValue("Gender", part.Gender);
            element.SetAttributeValue("TimeZone", part.TimeZone);
            element.SetAttributeValue("Locale", part.Locale);
            element.SetAttributeValue("IsVerified", part.IsVerified);
        }

        protected override void Importing(FacebookUserPart part, ImportContentContext context)
        {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "FacebookUserId", value => part.FacebookUserId = long.Parse(value));
            context.ImportAttribute(partName, "Name", value => part.Name = value);
            context.ImportAttribute(partName, "FirstName", value => part.FirstName = value);
            context.ImportAttribute(partName, "LastName", value => part.LastName = value);
            context.ImportAttribute(partName, "Link", value => part.Link = value);
            context.ImportAttribute(partName, "FacebookUserName", value => part.FacebookUserName = value);
            context.ImportAttribute(partName, "Gender", value => part.Gender = value);
            context.ImportAttribute(partName, "TimeZone", value => part.TimeZone = int.Parse(value));
            context.ImportAttribute(partName, "Locale", value => part.Locale = value);
            context.ImportAttribute(partName, "IsVerified", value => part.IsVerified = bool.Parse(value));
        }
    }
}