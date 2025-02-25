using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        doan5Entities database = new doan5Entities();
        // GET: Admin/Category
        public ActionResult Index()
        {
            var categories = database.Categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            try
            {
                database.Categories.Add(category);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("LOI TAO MOI CATEGORY");
            }
        }
        public ActionResult Details(int id)
        {
            var category = database.Categories.Where(c => c.ID == id).FirstOrDefault();
            return View(category);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = database.Categories.Where(c => c.ID == id).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(int id, Category category)
        {
            database.Entry(category).State = System.Data.Entity.EntityState.Modified; //Dòng mã cơ sở dữ liệu.Entry(category).State = System.Data.Entity.EntityState.Modified;
                                                                                      //được sử dụng trong Entity Framework để đánh dấu một thực thể là đã sửa đổi.
                                                                                      //Điều này có nghĩa là thực thể đã được thay đổi kể từ lần tải cuối cùng từ cơ sở dữ liệu
                                                                         //và Entity Framework đó sẽ tạo ra một câu lệnh UPDATE để cập nhật thực thể trong cơ sở dữ liệu
                                                                                      //khi phương thức SaveChanges() được gọi.
            database.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = database.Categories.Where(c => c.ID == id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var category = database.Categories.Where(c => c.ID == id).FirstOrDefault();
                if (category == null)
                {
                    return HttpNotFound();
                }

                database.Categories.Remove(category); // Xóa category từ database
                database.SaveChanges(); // Lưu thay đổi vào database
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Không thể xóa do liên quan đến các bảng khác");
            }
        }
    }
}
