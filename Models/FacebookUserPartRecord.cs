using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Records;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookUserPartRecord : ContentPartRecord
    {
        public virtual long FacebookUserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Link { get; set; }
        public virtual string FacebookUserName { get; set; }
        public virtual string Gender { get; set; }
        public virtual int TimeZone { get; set; }
        public virtual string Locale { get; set; }
        public virtual bool IsVerified { get; set; }
    }
}