using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    
    public class ProductReviewsController : Controller
    {
        private doan5Entities db = new doan5Entities();

        // GET: Admin/ProductReviews (Hiển thị danh sách đánh giá theo sản phẩm)
        public ActionResult Index()
        {
            var groupedReviews = db.ProductReviews
                                  .GroupBy(r => r.Product)
                                  .Select(g => new ProductReviewGroup
                                  {
                                      Product = g.Key,
                                      Reviews = g.ToList()
                                  }).ToList();
            return View(groupedReviews);
        }

        // POST: Admin/ProductReviews/Delete/5 (Xóa đánh giá)
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ProductReview review = db.ProductReviews.Find(id);
            if (review != null)
            {
                db.ProductReviews.Remove(review);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Admin/ProductReviews/Reply (Phản hồi đánh giá)
       
    }

    public class ProductReviewGroup
    {
        public Product Product { get; set; }
        public List<ProductReview> Reviews { get; set; }
    }
}
