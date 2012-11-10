using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Piedone.Facebook.Suite.Connect
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("FacebookConnect").SetUrl("piedone-facebook-suite-connect.js").SetDependencies("FacebookSuite");
            manifest.DefineStyle("FacebookConnect").SetUrl("piedone-facebook-suite-connect.css");
        }
    }
}
