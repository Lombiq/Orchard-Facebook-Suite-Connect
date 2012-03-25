using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;
using Orchard.ContentManagement.Handlers;

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

        protected override void Exporting(FacebookUserPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FacebookUserId", part.FacebookUserId);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
            context.Element(part.PartDefinition.Name).SetAttributeValue("FirstName", part.FirstName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("LastName", part.LastName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Link", part.Link);
            context.Element(part.PartDefinition.Name).SetAttributeValue("FacebookUserName", part.FacebookUserName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Gender", part.Gender);
            context.Element(part.PartDefinition.Name).SetAttributeValue("TimeZone", part.TimeZone);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Locale", part.Locale);
            context.Element(part.PartDefinition.Name).SetAttributeValue("IsVerified", part.IsVerified);
        }

        protected override void Importing(FacebookUserPart part, ImportContentContext context)
        {
            part.FacebookUserId = long.Parse(context.Attribute(part.PartDefinition.Name, "FacebookUserId"));
            part.Name = context.Attribute(part.PartDefinition.Name, "Name");
            part.FirstName = context.Attribute(part.PartDefinition.Name, "FirstName");
            part.LastName = context.Attribute(part.PartDefinition.Name, "LastName");
            part.Link = context.Attribute(part.PartDefinition.Name, "Link");
            part.FacebookUserName = context.Attribute(part.PartDefinition.Name, "FacebookUserName");
            part.Gender = context.Attribute(part.PartDefinition.Name, "Gender");
            part.TimeZone = int.Parse(context.Attribute(part.PartDefinition.Name, "TimeZone"));
            part.Locale = context.Attribute(part.PartDefinition.Name, "Locale");
            part.IsVerified = bool.Parse(context.Attribute(part.PartDefinition.Name, "IsVerified"));
        }
    }
}