using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Products_Catalog;
using Products_Catalog.Controllers;
using Products_Catalog.Controllers.api;
using Products_Catalog.Models;
using Newtonsoft.Json;



namespace Products_CatalogTest
{
    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        public void TestCreateProduct()
        {
            Product productTest = new Product(0, "TestCode", "TestName", "", 100, default);
            ValuesController valuesControllerTest = new ValuesController();
            string result = valuesControllerTest.CreateProduct(productTest);
            CrudResult crudResult = JsonConvert.DeserializeObject<CrudResult>(result);
            if (crudResult.id > 0)
                valuesControllerTest.DeleteProduct(crudResult.id);
            Assert.AreEqual("OK", crudResult.Result);
        }
    }
}
