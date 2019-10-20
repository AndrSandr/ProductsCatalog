using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Products_Catalog.Models
{
    public class ProductContext : DbContext
    {
        public ProductContext()
           : base("DbConnection")
        { }

        public DbSet<Product> Products { get; set; }
    }
}