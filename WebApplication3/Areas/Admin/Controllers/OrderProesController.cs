using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Controllers
{
    public class OrderProesController : Controller
    {
        private doan5Entities db = new doan5Entities();

        // GET: Admin/OrderProes
        public ActionResult Index()
        {
            var orderProes = db.OrderProes.Include(o => o.Customer).Include(o => o.Voucher).ToList();

            // Tính toán tổng tiền nếu cần thiết
            foreach (var order in orderProes)
            {
                // Tính tổng tiền (TotalPrice) từ OrderDetails nếu nó chưa có trong cơ sở dữ liệu
                var totalPrice = db.OrderDetails.Where(od => od.IDOrder == order.ID)
                                                .Sum(od => od.SubTotal);
                order.TotalPrice = totalPrice;
                db.SaveChanges();
            }

            return View(orderProes);
        
        }


        // GET: Admin/OrderProes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }

        // GET: Admin/OrderProes/Create
        public ActionResult Create()
        {
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus");
            ViewBag.VoucherID = new SelectList(db.Vouchers, "VoucherID", "VoucherCode");
            return View();
        }

        // POST: Admin/OrderProes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DateOrder,IDCus,AddressDelivery,StatusOrder,TotalPrice,PaymentMethod,VoucherID,Note")] OrderPro orderPro)
        {
            if (ModelState.IsValid)
            {
                db.OrderProes.Add(orderPro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            ViewBag.VoucherID = new SelectList(db.Vouchers, "VoucherID", "VoucherCode", orderPro.VoucherID);
            return View(orderPro);
        }

        // GET: Admin/OrderProes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy đơn hàng theo ID
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách khách hàng và voucher để hiển thị trên dropdown
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            ViewBag.VoucherID = new SelectList(db.Vouchers, "VoucherID", "VoucherCode", orderPro.VoucherID);

            // Thêm danh sách trạng thái cho dropdown
            ViewBag.StatusOrderList = new SelectList(new[] { "Đang xử lý", "Đang giao", "Đã giao" }, orderPro.StatusOrder);

            return View(orderPro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DateOrder,IDCus,AddressDelivery,StatusOrder,TotalPrice,PaymentMethod,VoucherID,Note")] OrderPro orderPro)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật trạng thái của đơn hàng
                db.Entry(orderPro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Gửi lại dữ liệu dropdown trong trường hợp có lỗi
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            ViewBag.VoucherID = new SelectList(db.Vouchers, "VoucherID", "VoucherCode", orderPro.VoucherID);
            ViewBag.StatusOrderList = new SelectList(new[] { "Đang xử lý", "Đang giao", "Đã giao" }, orderPro.StatusOrder);

            return View(orderPro);
        }


        // GET: Admin/OrderProes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }

        // POST: Admin/OrderProes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }

            var orderDetails = db.OrderDetails.Where(od => od.IDOrder == id).ToList();
            db.OrderDetails.RemoveRange(orderDetails);
            db.OrderProes.Remove(orderPro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       

    }
}
