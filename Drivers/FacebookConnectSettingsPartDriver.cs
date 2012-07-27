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
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("Permissions", part.Permissions);
            element.SetAttributeValue("OnlyAllowVerified", part.OnlyAllowVerified);
            element.SetAttributeValue("SimpleRegistration", part.SimpleRegistration);
        }

        protected override void Importing(FacebookConnectSettingsPart part, ImportContentContext context)
        {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "Permissions", value => part.Permissions = value);
            context.ImportAttribute(partName, "OnlyAllowVerified", value => part.OnlyAllowVerified = bool.Parse(value));
            context.ImportAttribute(partName, "SimpleRegistration", value => part.SimpleRegistration = bool.Parse(value));
        }
    }
}