using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Products_Catalog.Models
{
    public class Product
    {
       
        public Product(int id, String code, String name, String photo, Double price, DateTime lastupdate)
        {
            Id = id;
            Code = code;
            Name = name;
            Photo = photo;
            Price =price;
            LastUpdate = lastupdate;
        }

        public Product() { }

        public int Id { get; set; }
        [DisplayAttribute(Name = "Code")]
        [Required()]
        public String Code { get; set; }
        [DisplayAttribute(Name = "Name")]
        [Required()]
        public String Name { get; set; }
        [DisplayAttribute(Name = "Photo")]
        public String Photo { get; set; }
        [DisplayAttribute(Name = "Price")]
        [Required()]
        public Double Price { get; set; }
        [DisplayAttribute(Name = "LastUpdate")]
        [Required()]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime LastUpdate { get; set; }
       
    }
}