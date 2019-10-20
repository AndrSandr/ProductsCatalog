using Products_Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Data.Entity;
using Swashbuckle.Swagger.Annotations;

namespace Products_Catalog.Controllers.api
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        
        // GET api/<controller>
        /// <summary>
        /// Returns list of products.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "", typeof(List<Product>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<Product> GetProducts()
        {
            List<Product> productsList = new List<Product>();

            using (ProductContext db = new ProductContext())
            {
                var products = db.Products.OrderBy(p => p.Name);
                foreach (Product prod in products) {
                   if (prod.Photo.Trim().Length > 0)
                    {
                        String url = Request.RequestUri.ToString();
                        url = url.Replace("api/values", "Content/imgs");
                        prod.Photo = url + "/" + prod.Photo;
                    }
                   productsList.Add(prod);
                }

            }

            return productsList;
        }

        /// <summary>
        /// Returns one product by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "", typeof(Product))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public Product GetProduct(int id)
        {
            using (ProductContext db = new ProductContext())
            {
                Product product = db.Products.Find(id);
                if ((product.Photo .Trim().Length > 0)) {
                    String url = Request.RequestUri.ToString();

                    String[] urlParts = url.Split('/');
                    if (urlParts.Length > 0)
                    {
                        string partFinish = urlParts[urlParts.Length - 1];
                        if (Int32.TryParse(partFinish, out int j))
                        {
                            url = url.Replace("/" + partFinish, "");
                        }
                    }

                    url = url.Replace("api/values", "Content/imgs");
                    product.Photo = url + "/" + product.Photo;
                }
               

                return product;
            }
                
        }

       /// <summary>
       /// Creats product (with photo or without photo). Needs PRODUCT as parameter.
       /// </summary>
       /// <param name="product"></param>
       /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, "", typeof(CrudResult))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public String CreateProduct([FromBody]Product product)
        {
            CrudResult result = new CrudResult("OK");
            if (product.Price <= 0)
            {
                result.Result = "Price must be higher then 0";
            } else
            {
                if (product.Name.Trim().Length == 0 )
                {
                    result.Result = "Need product name.";
                } else
                {
                    if (product.Code.Trim().Length == 0)
                    {
                        result.Result = "Need product code.";
                    } else {
                        using (ProductContext db = new ProductContext())
                        {
                            var prods = db.Products.Where(p => p.Code == product.Code);
                            if (prods.Count() > 0)
                            {
                                result.Result = "Product code " + product .Code + " exists in database.";
                            } else
                            {
                                String filename = "";
                               filename = saveFile(product.Photo, product.Code);
                               if (filename.StartsWith ("Error."))
                               {
                                    result.Result = filename.Replace("Error. ", "");
                               } else
                                {
                                    product.Photo = filename;
                                    product.LastUpdate = DateTime.Now;
                                    db.Products.Add(product);
                                    db.SaveChanges();
                                    result.id = product.Id;
                                }

                            }

                        }
                            
                     }
                 }
                   
             }

            //}

            return JsonConvert.SerializeObject(result);
               
        }
        /// <summary>
        /// Deletes product by its ID. Void. Nothing returns.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void DeleteProduct(int id)
        {
           using (ProductContext db = new ProductContext())
            {
                Product product = db.Products.Find(id);
                if (product != null)
                {
                    String filename = product.Photo;
                    if (filename.Trim().Length > 0)
                    {
                        string filepath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(MvcApplication.PRODUCTS_PHOTO_PATH), filename);
                        try
                        {
                            File.Delete(filepath);
                        }
                        catch { }
                    }
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
                
            }

         }

        /// <summary>
        /// Updates product information. Needs product ID and PRODUCT as parameters. Void. Nothing returns.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void EditProduct(int id, [FromBody]Product product)
        {
            if (id == product.Id)
            {
                String tempPhoto = "";  //Need for saving old photo
                Boolean needRestorePhoto = false; //Need for saving old photo

                using (ProductContext db = new ProductContext())
                {
                   
                    var tempProducts = db.Products.Where(p => p.Id == product.Id);
                    foreach (Product tempProd in tempProducts)
                    {
                        tempPhoto = tempProd.Photo;
                    }
                }

                    if (product.Price <= 0)
                {
                    return;
                }
                else
                {
                    if (product.Name.Trim().Length == 0)
                    {
                        return;
                    }
                    else
                    {
                        if (product.Code.Trim().Length == 0)
                        {
                            return;
                        }
                        else
                        {
                            using (ProductContext db = new ProductContext())
                            {
                                var prods = db.Products.Where(p => p.Id != product.Id).Where(p => p.Code == product.Code);
                                if (prods.Count() > 0)
                                {
                                    return;
                                }
                                else
                                {
                                    String filename = "";
                                    if (product.Photo.Trim().Length == 0)
                                    {
                                        needRestorePhoto = true;
                                    } else
                                    {
                                        filename = saveFile(product.Photo, product.Code);
                                        if (filename.StartsWith("Error."))
                                        {
                                            product.Photo = "";
                                        }
                                        else
                                        {
                                            product.Photo = filename;

                                        }
                                    }
                                   
                                    product.LastUpdate = DateTime.Now;
                                    //db.Products.Add(product);

                                    db.Entry(product).State = EntityState.Modified;
                                    db.SaveChanges();


                                }

                            }
                            
                            if (needRestorePhoto)
                            {
                                using (ProductContext db = new ProductContext())
                                {
                                    System.Data.SqlClient.SqlParameter paramID = new System.Data.SqlClient.SqlParameter("@id", product.Id);
                                    System.Data.SqlClient.SqlParameter paramPhoto = new System.Data.SqlClient.SqlParameter("@photo", tempPhoto);
                                    int numberOfRowUpdated = db.Database.ExecuteSqlCommand("UPDATE Products SET Photo = @photo WHERE Id = @id", paramID, paramPhoto);
                                }
                            }
                           
                        }
                    }

                }
            }
               
        }

        private String saveFile(String productsPhoto, String productCode)
        {
            String ret = "";
            if (productsPhoto.Trim().Length > 0)
            {
                String[] fstrarr = productsPhoto.Split(',');
                String fileasstring = "";
                String filext = "jpg";
                if (fstrarr.Length > 1)
                {
                    fileasstring = fstrarr[fstrarr.Length - 1];
                    string[] fstrarr2 = fstrarr[0].Split(';');
                    if (fstrarr2.Length > 1)
                    {
                        var fstrarr3 = fstrarr2[0].Split('/');
                        if (fstrarr3.Length > 1)
                            filext = fstrarr3[1].ToString();
                    }
                }

                if (!(filext == "png") & !(filext == "jpg") & !(filext == "jpeg"))
                {
                    ret = "Error. Picture must be jpeg or png format.";
                }
                else
                {
                    if (filext == "jpeg") { filext = "jpg"; }
                    String filename = "product_" + productCode + "." + filext;
                    ret = filename;
                    var b = Convert.FromBase64String(fileasstring);
                    string filepath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(MvcApplication.PRODUCTS_PHOTO_PATH), filename);
                    try
                    {
                        File.Delete(filepath);
                    }
                    catch { }
                    
                    Image img;
                    using (var ms = new MemoryStream(b))
                    {
                        img = Image.FromStream(ms);
                        img.Save(filepath);
                    }


                }
            }
            return ret;
        }


    }

    public class CrudResult
    {

        public String Result { get; set; }
        public int id { get; set; }

        public CrudResult(String result)
        {
            Result = result;
            id = 0;
        }

    }

}