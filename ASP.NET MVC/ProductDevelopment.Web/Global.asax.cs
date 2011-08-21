using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using ProductDevelopment.Models;
using ProductDevelopment.Web.Infrastructure;
using ProductDevelopment.Web.Infrastructure.Data;
using ProductDevelopment.Web.Infrastructure.Security;
using ProductDevelopment.Web.Models;

namespace ProductDevelopment.Web
{
    public class MvcApplication : HttpApplication
    {
        private IKernel kernel; 
        protected void Application_Start()
        {
            kernel = new StandardKernel();

            Database.SetInitializer(new SeedData()); //Do not include in release/production

            AreaRegistration.RegisterAllAreas();
            RegisterDependencyResolver(kernel);
            RegisterGlobalFilters(GlobalFilters.Filters, kernel); 
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters, IKernel kernel)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(kernel.TryGet(typeof(UserProviderFilterAttribute)));
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        private static void RegisterDependencyResolver(IKernel kernel)
        {
            kernel.Bind<IAuthentication>().To<Authentication>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
            kernel.Bind<IRepository<Defect>>().To<Repository<Defect>>().InRequestScope();
            kernel.Bind<IRepository<Project>>().To<Repository<Project>>().InRequestScope();
            kernel.Bind<IRepository<Severity>>().To<Repository<Severity>>().InRequestScope();
            kernel.Bind<UserProviderFilterAttribute>().To<UserProviderFilterAttribute>();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}