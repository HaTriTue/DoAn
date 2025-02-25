using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class UsersController : Controller
    {
        private doan5Entities database = new doan5Entities();
        // GET: Users
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer cust)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra các trường không được để trống
                if (string.IsNullOrEmpty(cust.NameCus)) ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(cust.PassCus)) ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (string.IsNullOrEmpty(cust.EmailCus)) ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(cust.PhoneCus)) ModelState.AddModelError(string.Empty, "Điện thoại không được để trống");
                if (string.IsNullOrEmpty(cust.AddressCus)) ModelState.AddModelError(string.Empty, "Địa chỉ không được để trống");

                // Kiểm tra định dạng email phải chứa "@gmail.com"
                if (!cust.EmailCus.EndsWith("@gmail.com"))
                {
                    ModelState.AddModelError(string.Empty, "Email phải có định dạng @gmail.com");
                }

                // Kiểm tra mật khẩu có ít nhất 8 ký tự
                if (cust.PassCus.Length < 8)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu phải có ít nhất 8 ký tự");
                }

                // Kiểm tra số điện thoại phải có đúng 10 chữ số và chỉ chứa số
                if (!System.Text.RegularExpressions.Regex.IsMatch(cust.PhoneCus, @"^\d{10}$"))
                {
                    ModelState.AddModelError(string.Empty, "Số điện thoại phải có đúng 10 chữ số");
                }

                // Kiểm tra xem có người nào đã đăng ký với tên đăng nhập này hay chưa
                var khachhang = database.Customers.FirstOrDefault(k => k.NameCus == cust.NameCus);
                if (khachhang != null)
                {
                    ModelState.AddModelError(string.Empty, "Đã có người đăng ký tên này");
                }

                // Nếu tất cả điều kiện hợp lệ thì thêm vào cơ sở dữ liệu
                if (ModelState.IsValid)
                {
                    database.Customers.Add(cust);
                    database.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            return View();
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Customer cust)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(cust.NameCus))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(cust.PassCus))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");

                if (ModelState.IsValid)
                {
                    // Tìm khách hàng có tên đăng nhập và mật khẩu hợp lệ trong cơ sở dữ liệu
                    var khachhang = database.Customers.FirstOrDefault(k => k.NameCus == cust.NameCus && k.PassCus == cust.PassCus);
                    if (khachhang != null)
                    {
                        // Lưu đối tượng khách hàng vào session
                        Session["TaiKhoan"] = khachhang;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
            }
            return View("Login");
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Remove("TaiKhoan");

            return RedirectToAction("Index", "Home");
        }


        // GET: Users/Index
        public ActionResult Index()
        {
            if (Session["TaiKhoan"] != null)
            {
                var khachhang = Session["TaiKhoan"] as Customer;
                if (khachhang != null)
                {
                    return View(khachhang);
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng (ví dụ: ghi nhật ký vấn đề hoặc chuyển hướng)
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                // Nếu không có phiên đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Users");
            }
        }

        // GET: Users/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = database.Customers.FirstOrDefault(k => k.NameCus == userName);
                if (user != null)
                {
                    return RedirectToAction("Edit", new { id = user.IDCus });
                }
            }
            ViewBag.ThongBao = "Tên đăng nhập không đúng.";
            return View();
        }

        // GET: Users/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer user = database.Customers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCus,NameCus,PhoneCus,EmailCus,PassCus,AddressCus")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var userInDb = database.Customers.Find(customer.IDCus);
                if (userInDb != null)
                {
                    userInDb.NameCus = customer.NameCus;
                    userInDb.PhoneCus = customer.PhoneCus;
                    userInDb.EmailCus = customer.EmailCus;
                    userInDb.AddressCus = customer.AddressCus;

                    // Cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
                    if (!string.IsNullOrEmpty(customer.PassCus))
                    {
                        userInDb.PassCus = customer.PassCus;
                    }

                    database.Entry(userInDb).State = EntityState.Modified;
                    database.SaveChanges();

                    // Đăng xuất người dùng sau khi thông tin đã được cập nhật
                    Session.Remove("TaiKhoan");

                    // Chuyển hướng đến trang đăng nhập
                    return RedirectToAction("Login", "Users");
                }
            }
            return View(customer);
        }

    }
}
