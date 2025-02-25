using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using WebApplication3.Models;

namespace WebApplication3.Models
{
    public class View
    {
        public IEnumerable<Product> Products { get; set; }
        public IPagedList<Product> PagedProducts { get; set; }
    }
}