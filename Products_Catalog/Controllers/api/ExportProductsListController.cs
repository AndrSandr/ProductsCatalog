using Products_Catalog.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using NPOI;
using NPOI.HSSF.UserModel;

namespace Products_Catalog.Controllers.api
{
    public class ExportProductsListController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Export list of products to Excel. If parameter ID is empty - Full list. If parameter ID contains word - filtered by name list.
        /// Returns Excel file URL.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "", typeof(HttpResponseMessage))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public String ExportProductsList([FromUri]String id = "")
        {

            String url = Request.RequestUri.ToString();
            String fileUrl = "";

            List<Product> productsList = new List<Product>();

            if (id.Trim().Length == 0) {
                productsList = getFullList();
                fileUrl = url.Replace("api/ExportProductsList/", "templates/");
                url = url.Replace("api/ExportProductsList/", "Content/imgs/");
            } else
            {
                productsList = getFilteredList(id);
                fileUrl = url.Replace("api/ExportProductsList/" + id, "templates/");
                url = url.Replace("api/ExportProductsList/" + id, "Content/imgs/");
                
            }

           

            String filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_products.xls";
            String filepath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/templates"), filename);
            String filepathtemplate = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/templates"), "template.xls");
            fileUrl = fileUrl + filename;

            var fs = new FileStream(filepathtemplate, FileMode.Open, FileAccess.Read);
            var templateWorkbook = new HSSFWorkbook(fs, true);
            NPOI.SS.UserModel.ISheet sheet = templateWorkbook.GetSheet("ProductsList");

            int i = 1;
            foreach (Product product in productsList)
            {
                NPOI.SS.UserModel.IRow dataRow = sheet.GetRow(i);

                NPOI.SS.UserModel.ICell cell0 = dataRow.GetCell(0);
                cell0.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell0.SetCellValue(product.Id);

                NPOI.SS.UserModel.ICell cell1 = dataRow.GetCell(1);
                cell1.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell1.SetCellValue(product.Code);

                
                NPOI.SS.UserModel.ICell cell2 = dataRow.GetCell(2);
                cell2.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell2.SetCellValue(product.Name);

                NPOI.SS.UserModel.ICell cell3 = dataRow.GetCell(3);
                cell3.SetCellType(NPOI.SS.UserModel.CellType.String);
                var tempURL = "";
                if (product.Photo.Trim().Length > 0 )
                {
                    tempURL = url + product.Photo;
                }
                cell3.SetCellValue(tempURL);

                NPOI.SS.UserModel.ICell cell4 = dataRow.GetCell(4);
                cell4.SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                cell4.SetCellValue(product.Price);

                NPOI.SS.UserModel.ICell cell5 = dataRow.GetCell(5);
                cell5.SetCellType(NPOI.SS.UserModel.CellType.String);
                cell5.SetCellValue(product.LastUpdate.ToString("yyyyMMdd HH:mm:ss"));

                i += 1;
            }


            try
            {
                File.Delete(filepath);
            }
            catch { }
            var memoryStream = new MemoryStream();
            templateWorkbook.Write(memoryStream);
            Byte[] content = memoryStream.ToArray();
            System.IO.File.WriteAllBytes(filepath, content);


            return fileUrl;
        }

       
        private List<Product> getFullList()
        {
            List<Product> productsList = new List<Product>();

            using (ProductContext db = new ProductContext())
            {
                var products = db.Products.OrderBy(p => p.Name);

                foreach (Product prod in products)
                {
                   productsList.Add(prod);
                }

            }

            return productsList;
        }

        private List<Product> getFilteredList(String filterName)
        {
            List<Product> productsList = new List<Product>();

            using (ProductContext db = new ProductContext())
            {
                var products = db.Products.Where(p => p.Name.Contains(filterName)).OrderBy(p => p.Name);

                foreach (Product prod in products)
                {
                   productsList.Add(prod);
                }

            }

            return productsList;
        }

    }
}