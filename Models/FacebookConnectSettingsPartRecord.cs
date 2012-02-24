using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Records;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectSettingsPartRecord : ContentPartRecord, IFacebookConnectSettings
    {
        public virtual string Permissions { get; set; }
        public virtual bool AutoLogin { get; set; }
        public virtual bool OnlyAllowVerified { get; set; }
        public virtual bool SimpleRegistration { get; set; }

        public FacebookConnectSettingsPartRecord()
        {
            Permissions = "";
            AutoLogin = false;
            OnlyAllowVerified = false;
            SimpleRegistration = false;
        }
    }
}