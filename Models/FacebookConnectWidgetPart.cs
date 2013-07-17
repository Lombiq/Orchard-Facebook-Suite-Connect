using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using Orchard.Environment.Extensions;

namespace Piedone.Facebook.Suite.Models
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectWidgetPart : ContentPart
    {
        private readonly LazyField<string> _permissions = new LazyField<string>();
        internal LazyField<string> PermissionsField { get { return _permissions; } }
        public string Permissions
        {
            get { return _permissions.Value; }
        }
    }
}