using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        private doan5Entities db = new doan5Entities();

        // Hiển thị danh sách báo cáo doanh thu
        public ActionResult Index(int? month, int? year)
        {
            if (!month.HasValue || !year.HasValue)
            {
                month = DateTime.Now.Month;
                year = DateTime.Now.Year;
            }

            var report = db.RevenueReports
                           .Where(r => r.ReportMonth == month && r.ReportYear == year)
                           .FirstOrDefault();

            if (report == null)
            {
                ViewBag.Message = "Không có dữ liệu báo cáo cho tháng và năm đã chọn.";
                return View(); // Trả về View mà không có Model để tránh lỗi null
            }

            return View(report);
        }


        // Tạo báo cáo doanh thu mới
        public ActionResult GenerateReport()
        {
            ViewBag.Month = new SelectList(Enumerable.Range(1, 12));
            ViewBag.Year = new SelectList(Enumerable.Range(DateTime.Now.Year - 5, 6));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateReport(int month, int year)
        {
            if (month < 1 || month > 12 || year < 2000)
            {
                ViewBag.Message = "Tháng hoặc năm không hợp lệ.";
                return RedirectToAction("GenerateReport");
            }

            try
            {
                var totalOrders = db.OrderProes.Count(o => o.DateOrder.Month == month && o.DateOrder.Year == year);
                var totalRevenue = db.OrderDetails
                                     .Where(od => db.OrderProes.Any(o => o.ID == od.IDOrder && o.DateOrder.Month == month && o.DateOrder.Year == year))
                                     .Sum(od => (decimal?)od.SubTotal) ?? 0;

                if (totalOrders == 0)
                {
                    ViewBag.Message = "Không có đơn hàng nào trong tháng và năm này.";
                    return RedirectToAction("GenerateReport");
                }

                // 🔥 **Lấy Top 1 khách hàng có tổng đơn hàng cao nhất**
                var topCustomer = db.OrderProes
                    .Where(o => o.DateOrder.Month == month && o.DateOrder.Year == year && o.StatusOrder == "Đã giao")
                    .GroupBy(o => o.Customer.NameCus)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "Không có khách hàng";

                // 🔥 **Lấy Top 1 sản phẩm bán chạy nhất**
                var topProduct = db.OrderDetails
                    .Where(od => db.OrderProes.Any(o => o.ID == od.IDOrder && o.DateOrder.Month == month && o.DateOrder.Year == year))
                    .GroupBy(od => od.Product.NamePro)
                    .OrderByDescending(g => g.Sum(od => od.Quantity))
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "Không có sản phẩm";

                var existingReport = db.RevenueReports.FirstOrDefault(r => r.ReportMonth == month && r.ReportYear == year);

                if (existingReport != null)
                {
                    existingReport.TotalOrders = totalOrders;
                    existingReport.TotalRevenue = totalRevenue;
                    existingReport.TopCustomer = topCustomer;
                    existingReport.TopProduct = topProduct;
                    existingReport.GeneratedAt = DateTime.Now;
                }
                else
                {
                    var report = new RevenueReport
                    {
                        ReportMonth = month,
                        ReportYear = year,
                        TotalRevenue = totalRevenue,
                        TotalOrders = totalOrders,
                        TopCustomer = topCustomer,
                        TopProduct = topProduct,
                        GeneratedAt = DateTime.Now
                    };

                    db.RevenueReports.Add(report);
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { month, year });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ViewBag.Message = "Lỗi khi lưu dữ liệu: " + string.Join("; ", errorMessages);
                return RedirectToAction("GenerateReport");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi: " + ex.Message;
                return RedirectToAction("GenerateReport");
            }
        }




        // Hiển thị danh sách Top 5 sản phẩm bán chạy nhất
        public ActionResult Top5BestSellingProducts()
        {
            List<OrderDetail> orderD = db.OrderDetails.ToList();
            List<Product> proList = db.Products.ToList();
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


        // Hiển thị danh sách Top 5 khách hàng có tổng đơn hàng cao nhất
        public ActionResult Top5Customers()
        {
            var query = db.OrderProes
                .Where(o => o.StatusOrder == "Đã giao") // Chỉ lấy đơn hàng đã hoàn thành
                .GroupBy(o => new { o.IDCus, o.Customer.NameCus, o.Customer.EmailCus, o.Customer.PhoneCus })
                .Select(gr => new TopCustomerViewModel
                {
                    CustomerID = gr.Key.IDCus,
                    Name = gr.Key.NameCus,
                    Email = gr.Key.EmailCus,
                    Phone = gr.Key.PhoneCus,
                    TotalOrders = gr.Count(), // Tổng số đơn hàng
                    TotalSpent = gr.Sum(o => (decimal?)o.TotalPrice) ?? 0 // Tổng tiền, tránh lỗi null
                })
                .OrderByDescending(c => c.TotalSpent) // Sắp xếp theo tổng tiền chi tiêu
                .Take(5)
                .ToList();

            return View(query);
        }



    }
}
