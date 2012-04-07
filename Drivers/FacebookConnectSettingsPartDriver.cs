using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Piedone.Facebook.Suite.Models;
using Orchard.ContentManagement.Handlers;

namespace Piedone.Facebook.Suite.Drivers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectSettingsPartDriver : ContentPartDriver<FacebookConnectSettingsPart>
    {
        public Localizer T { get; set; }

        protected override string Prefix
        {
            get { return "FacebookConnect"; }
        }

        // GET
        protected override DriverResult Editor(FacebookConnectSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_FacebookConnectSettings_SiteSettings",
                () =>
                {
                    return shapeHelper.EditorTemplate(
                        TemplateName: "Parts.FacebookConnectSettings.SiteSettings",
                        Model: part,
                        Prefix: Prefix);
                }).OnGroup("FacebookSuiteSettings");
        }

        // POST
        protected override DriverResult Editor(FacebookConnectSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Exporting(FacebookConnectSettingsPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Permissions", part.Permissions);
            context.Element(part.PartDefinition.Name).SetAttributeValue("OnlyAllowVerified", part.OnlyAllowVerified);
            context.Element(part.PartDefinition.Name).SetAttributeValue("SimpleRegistration", part.SimpleRegistration);
        }

        protected override void Importing(FacebookConnectSettingsPart part, ImportContentContext context)
        {
            part.Permissions = context.Attribute(part.PartDefinition.Name, "Permissions");
            part.OnlyAllowVerified = bool.Parse(context.Attribute(part.PartDefinition.Name, "OnlyAllowVerified"));
            part.SimpleRegistration = bool.Parse(context.Attribute(part.PartDefinition.Name, "SimpleRegistration"));
        }
    }
}