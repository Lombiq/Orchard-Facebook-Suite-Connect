using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectSettingsPart : ContentPart<FacebookConnectSettingsPartRecord>, IFacebookConnectSettings
    {
        public string Permissions
        {
            get { return Record.Permissions; }
            set { Record.Permissions = value; }
        }

        public bool AutoLogin
        {
            get { return Record.AutoLogin; }
            set { Record.AutoLogin = value; }
        }

        public bool OnlyAllowVerified
        {
            get { return Record.OnlyAllowVerified; }
            set { Record.OnlyAllowVerified = value; }
        }

        public bool SimpleRegistration
        {
            get { return Record.SimpleRegistration; }
            set { Record.SimpleRegistration = value; }
        }
    }
}