using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectWidgetPart : ContentPart
    {
        private readonly LazyField<string> _permissions = new LazyField<string>();
        public LazyField<string> PermissionsField { get { return _permissions; } }
        public string Permissions
        {
            get { return _permissions.Value; }
        }
    }
}