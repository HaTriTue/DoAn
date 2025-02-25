using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class OrderDetailController : Controller
    {
        doan5Entities database = new doan5Entities();
        // GET: OrderDetail

      
        public ActionResult GroupByTop5()
        {
            List<OrderDetail> orderD = database.OrderDetails.ToList();
            List<Product> proList = database.Products.ToList();
            var query = from od in orderD
                        join p in proList on od.IDProduct equals p.ProductID into tbl
                        group od by new { idPro = od.IDProduct, namePro = od.Product.NamePro, ImagePro = od.Product.ImagePro, price = od.Product.Price }
            into gr
                        orderby gr.Sum(s => s.Quantity) descending
                        select new ViewModel
                        {
                            IDPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = Url.Content(gr.Key.ImagePro), // Update to use Url.Content for the image path
                            pricePro = (decimal)gr.Key.price,
                            Sum_Quantity = gr.Sum(s => s.Quantity)
                        };
            return View(query.Take(5).ToList());
        }
    }
}