using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectSettingsPartRecord : ContentPartRecord, IFacebookConnectSettings
    {
        public virtual string Permissions { get; set; }
        public virtual bool OnlyAllowVerified { get; set; }
        public virtual bool SimpleRegistration { get; set; }

        public FacebookConnectSettingsPartRecord()
        {
            Permissions = "";
            OnlyAllowVerified = false;
            SimpleRegistration = false;
        }
    }
}