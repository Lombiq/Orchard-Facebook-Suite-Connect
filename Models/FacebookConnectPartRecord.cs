using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Records;

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
}