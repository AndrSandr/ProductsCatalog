using Products_Catalog.Models;
using Swashbuckle .Swagger;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Products_Catalog.Controllers.api
{

   
    public class FilteredListController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Returns filtered by part of name list of products.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "", typeof(List<Product>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IEnumerable<Product> GetProductsList([FromUri]string id="")
        {
            List<Product> productsList = new List<Product>();
            
            using (ProductContext db = new ProductContext())
            {
                // var products = db.Products.OrderBy(p => p.Name);
                var products = db.Products.Where(p => p.Name.ToLower().Contains (id.ToLower()));
                foreach (Product prod in products)
                {
                    if (prod.Photo.Trim().Length > 0)
                    {
                        String url = Request.RequestUri.ToString();
                        String prefix = "api/FilteredList/" + id;
                        url = url.Replace(prefix, "Content/imgs");
                        prod.Photo = url + "/" + prod.Photo;
                    }
                    productsList.Add(prod);
                }

            }

            return productsList;
        }

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}