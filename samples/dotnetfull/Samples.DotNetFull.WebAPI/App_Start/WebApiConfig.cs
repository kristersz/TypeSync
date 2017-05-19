using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Samples.DotNetFull.WebAPI.IoC;
using Samples.DotNetFull.WebAPI.Services;

namespace Samples.DotNetFull.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            container.RegisterType<IStudentService, StudentService>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // produce JSON
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
