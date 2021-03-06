﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportyGeek.WebUI.Infrastructure;
using System.Reflection;
using SportyGeek.Domain.NinjectModules;

namespace SportyGeek.WebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "AllProductsPaged", // Route name
                "Page{page}", // URL with parameters
                new { controller = "Products", action = "List", category = (string)null }, // Parameter defaults
                new { page = @"\d+" }
            );

            routes.MapRoute(
                "CategoryProducts", // Route name
                "{category}", // URL with parameters
                new { controller = "Products", action = "List", page = 1} // Parameter defaults
            );

            routes.MapRoute(
                "CategoryProductsPaged", // Route name
                "{category}/Page{page}", // URL with parameters
                new { controller = "Products", action = "List"}, // Parameter defaults
                new { page = @"\d+" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Products", action = "List" } // Parameter defaults
            );

            routes.MapRoute(
                "AllProducts", // Route name
                "", // URL with parameters
                new { controller = "Products", action = "List", category = (string)null, page = 1 }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            IKernel kernel = new StandardKernel(
                new NinjectControllerModule(Assembly.GetExecutingAssembly()),
                new NHibernateRepositoryModule()
            );
            // create one ISession per request, and it's only created when someone asks for one.
            kernel.Bind<NHibernate.ISession>().ToMethod(c => c.Kernel.Get<NHibernate.ISessionFactory>().OpenSession()).InRequestScope();

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(kernel));

            ViewEngines.Engines.Add(new SixBit.Web.CsJs.CsJsViewEngine());
        }
    }
}