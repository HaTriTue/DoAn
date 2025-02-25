using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication3.Models;
using System.Net;
using PagedList;
using PagedList.Mvc;

namespace WebApplication3.Controllers
{
    public class CustomerProductsController : Controller
    {
        private doan5Entities db = new doan5Entities();
        // GET: CustomerProducts
        public ActionResult Index(string category, int? page, double min = double.MinValue, double max = double.MinValue, string Searching = "")
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);

            var productList = db.Products.AsQueryable(); // Lấy danh sách sản phẩm từ DbContext

            if (!string.IsNullOrEmpty(Searching))
            {
                productList = productList.Where(x => x.NamePro.ToUpper().Contains(Searching.ToUpper()));
            }

            if (category != null)
            {
                int categoryId = int.Parse(category); // Chuyển đổi string sang int
                productList = productList.Where(p => p.CategoryID == categoryId);
            }


            // Sắp xếp sản phẩm theo tên hoặc điều kiện tùy chọn khác nếu cần
            productList = productList.OrderBy(x => x.NamePro);

            // Trả về trang sản phẩm sử dụng PagedList
            return View(productList.ToPagedList(pageNum, pageSize));
        }


        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.ProductReviews.Select(r => r.Customer)) // Load cả Customer
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }


        public ActionResult GetProductsByCategory()
        {
            var categories = db.Categories.ToList();
            return PartialView("CategoriesPartialView", categories);
        }
        public ActionResult GetProductsByCateId(int id)
        {
            var products = db.Products.Where(p => p.Category.ID == id).ToList();
            return View("Index", products);
        }
        [HttpPost]
        public ActionResult SubmitReview(int productId, string reviewText, int rating)
        {
            // Kiểm tra khách hàng có đang đăng nhập không
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin khách hàng từ session
            Customer khach = Session["TaiKhoan"] as Customer;
            if (khach == null)
                return RedirectToAction("Login", "Users");

            // Tạo đánh giá mới
            var review = new ProductReview
            {
                ProductID = productId,
                IDCus = khach.IDCus, // Lấy ID từ đối tượng khách hàng
                ReviewText = reviewText,
                Rating = rating,
                ReviewDate = DateTime.Now
            };

            // Lưu vào database
            db.ProductReviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = productId });
        }


    }
}