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
    public class OrderDetailsController : Controller
    {
        private doan5Entities db = new doan5Entities();

        // GET: Admin/OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.OrderPro).Include(o => o.Product);
            return View(orderDetails.ToList());
        }

        // GET: Admin/OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderDetail orderDetail = db.OrderDetails.Find(id);  // Lấy chi tiết đơn hàng theo ID

            if (orderDetail == null)
            {
                return HttpNotFound();
            }

            return View(orderDetail);  // Trả về view với đối tượng OrderDetail
        }



        // GET: Admin/OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.IDOrder = new SelectList(db.OrderProes, "ID", "AddressDelivery");
            ViewBag.IDProduct = new SelectList(db.Products, "ProductID", "NamePro");
            return View();
        }

        // POST: Admin/OrderDetails/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IDProduct,IDOrder,Quantity,UnitPrice,DiscountAmount,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                orderDetail.SubTotal = orderDetail.Quantity * orderDetail.UnitPrice;
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDOrder = new SelectList(db.OrderProes, "ID", "AddressDelivery", orderDetail.IDOrder);
            ViewBag.IDProduct = new SelectList(db.Products, "ProductID", "NamePro", orderDetail.IDProduct);
            return View(orderDetail);
        }



        // GET: Admin/OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDOrder = new SelectList(db.OrderProes, "ID", "AddressDelivery", orderDetail.IDOrder);
            ViewBag.IDProduct = new SelectList(db.Products, "ProductID", "NamePro", orderDetail.IDProduct);
            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IDProduct,IDOrder,Quantity,UnitPrice,DiscountAmount,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật SubTotal
                orderDetail.SubTotal = orderDetail.Quantity * orderDetail.UnitPrice;

                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDOrder = new SelectList(db.OrderProes, "ID", "AddressDelivery", orderDetail.IDOrder);
            ViewBag.IDProduct = new SelectList(db.Products, "ProductID", "NamePro", orderDetail.IDProduct);
            return View(orderDetail);
        }


        // GET: Admin/OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
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
