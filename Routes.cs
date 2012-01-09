using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Piedone.Facebook.Suite.Connect
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Name = "FacebookConnect",
                    Route = new Route(
                        "FacebookConnect",
                        new RouteValueDictionary {
                                                    {"area", "Piedone.Facebook.Suite.Connect"},
                                                    {"controller", "Connect"},
                                                    {"action", "Connect"}
                                                },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                                                    {"area", "Piedone.Facebook.Suite.Connect"}
                                                },
                        new MvcRouteHandler())
                }
            };
        }
    }
}