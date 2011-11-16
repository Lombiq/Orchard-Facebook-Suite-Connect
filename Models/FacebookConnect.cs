using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectPartRecord : ContentPartRecord
    {
        public virtual string Permissions { get; set; }
        public virtual bool AutoLogin { get; set; }
        public virtual bool OnlyAllowVerified { get; set; }

        public FacebookConnectPartRecord()
        {
            Permissions = "";
            AutoLogin = false;
            OnlyAllowVerified = false;
        }
    }

    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectPart : ContentPart<FacebookConnectPartRecord>
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
    }
}