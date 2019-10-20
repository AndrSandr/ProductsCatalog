using Products_Catalog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Products_Catalog.Controllers
{
    public class RemovePictureController : Controller
    {
        // GET: RemovePicture
        public ActionResult Index(String code)
        {
            using (ProductContext db = new ProductContext())
            {
                var products = db.Products.Where(p => p.Code == code);
                foreach (Product product in products)
                {
                    String filename = product.Photo;
                    if (filename.Trim().Length > 0)
                    {
                        string filepath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(MvcApplication.PRODUCTS_PHOTO_PATH), filename);
                        try
                        {
                            System.IO.File.Delete(filepath);
                        }
                        catch { }
                    }
                   

                }


            }
            using (ProductContext db = new ProductContext())
            {
                System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter("@code", code);
                int numberOfRowUpdated = db.Database.ExecuteSqlCommand("UPDATE Products SET Photo = '' WHERE Code=@code", param);
            }
                return Json("{OK}");
            
        }

       
    }

   
}