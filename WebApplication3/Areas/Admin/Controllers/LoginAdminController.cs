using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class LoginAdminController : Controller
    {
        private doan5Entities database = new doan5Entities();

        // Đăng ký Admin
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Adminuser ad)
        {
            if (ad == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu không hợp lệ.");
                return View();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ad.NameUser)) ModelState.AddModelError(string.Empty, "Tên không được để trống");
                if (string.IsNullOrEmpty(ad.RoleUser)) ModelState.AddModelError(string.Empty, "Vai trò không được để trống");
                if (string.IsNullOrEmpty(ad.PasswordUser)) ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");

                // Kiểm tra xem tên đã tồn tại chưa
                var admin = database.Adminusers.FirstOrDefault(k => k.NameUser == ad.NameUser);
                if (admin != null) ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại");

                if (ModelState.IsValid)
                {
                    // Hash mật khẩu trước khi lưu
                    ad.PasswordUser = HashPassword(ad.PasswordUser);

                    database.Adminusers.Add(ad);
                    database.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }



        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Adminuser ad)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ad.NameUser)) ModelState.AddModelError(string.Empty, "Tên không được để trống");
                if (string.IsNullOrEmpty(ad.PasswordUser)) ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");

                if (ModelState.IsValid)
                {
                    // Lấy tài khoản từ database
                    var admin = database.Adminusers.FirstOrDefault(k => k.NameUser == ad.NameUser);
                    if (admin != null && VerifyPassword(ad.PasswordUser, admin.PasswordUser))
                    {
                        ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công!";
                        Session["TaiKhoan"] = admin;
                        return RedirectToAction("Index", "MainAdmin", new { area = "Admin" });
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    }
                }
            }
            return View("Login");
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("TaiKhoan");
            return RedirectToAction("Index", "MainAdmin", new { area = "Admin" });
        }

        // Hàm băm mật khẩu (Hash Password)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Hàm kiểm tra mật khẩu (Verify Password)
        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            string enteredHashed = HashPassword(enteredPassword);
            return enteredHashed == storedHashedPassword;
        }
    }
}
