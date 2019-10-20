using Products_Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Products_Catalog
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public const String PRODUCTS_PHOTO_PATH = "~/Content/imgs";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           

            //Entity framework Creating Database
            using (ProductContext db = new ProductContext())
            {

                //Checking if database exists-------
                int productsCount = 0;
                var products = db.Products;
                try
                {
                    productsCount = products.Count();
                } catch { productsCount = 0; }

                //----------------------------------

                if (productsCount == 0) {
                    Product productFirst = new Product(-1, "HeadAndShoulders", "Head And Shoulders", "hs.jpg", 10.01, DateTime.Now);
                    Product productSecond = new Product(-1, "Shamptu", "Shamtu", "shamtu.png", 08.00, DateTime.Now);

                    db.Products.Add(productFirst);
                    db.Products.Add(productSecond);

                    db.SaveChanges();
                }
               
            }

            //-------------------------------------------------
        }
    }
}
