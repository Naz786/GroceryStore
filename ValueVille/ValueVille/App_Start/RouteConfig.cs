using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ValueVille.Models;

namespace ValueVille
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add("CategoryIndex", 
                     new SeoFriendlyRoute("home/category/{id}/",
                     new RouteValueDictionary(new { controller = "Home", action = "Products" }),
                     new MvcRouteHandler()));

            routes.Add("ProductIndex", 
                     new SeoFriendlyRoute("product/index/{id}/",
                     new RouteValueDictionary(new { controller = "Product", action = "Index" }),
                     new MvcRouteHandler()));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
