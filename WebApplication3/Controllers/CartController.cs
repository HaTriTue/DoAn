using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class CartController : Controller
    {
       
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        public List<CartItem> GetCart()
        {
            List<CartItem> myCart = Session["GioHang"] as
            List<CartItem>;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (myCart == null)
            {
                myCart = new List<CartItem>();
                Session["GioHang"] = myCart;
            }
            return myCart;
        }
        public ActionResult AddToCart(int id)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("ConfirmCart", "Cart", new
            {
                id = id
            });
        }
        public ActionResult AddToCart2(int id)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("GetCartInfo", "Cart", new
            {
                id = id
            });
        }
        private int GetTotalNumber()
        {
            int totalNumber = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalNumber = myCart.Sum(sp => sp.Number);
            return totalNumber;
        }
        private decimal GetTotalPrice()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalPrice = myCart.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }
        public ActionResult GetCartInfo()
        {
            List<CartItem> myCart = GetCart();
            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (myCart == null || myCart.Count == 0)
            {
                return RedirectToAction("Index", "CustomerProducts");
            }
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
          
            return View(myCart); //Trả về View hiển thị thông tin giỏ hàng
        }
        public ActionResult DeleteCartItem(int id)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                myCart.RemoveAll(p => p.ProductID == id);
                return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
            }
            if (myCart.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                return RedirectToAction("Index", "CustomerProducts");
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult UpdateCartItem(int id, int Number)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                //Cập nhật lại số lượng tương ứng
                //Lưu ý số lượng phải lớn hơn hoặc bằng 1
                currentProduct.Number = Number;
            }
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult ConfirmCart(string orderNote)
        {
            if (Session["TaiKhoan"] == null) // Chưa đăng nhập
                return RedirectToAction("Login", "Users");

            List<CartItem> myCart = GetCart();
            if (myCart == null || myCart.Count == 0) // Chưa có giỏ hàng hoặc chưa có sản phẩm
                return RedirectToAction("Index", "CustomerProducts");

            // Lưu ghi chú đơn hàng vào TempData
            TempData["OrderNote"] = orderNote;

            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();

            return View(myCart); // Trả về View hiển thị thông tin giỏ hàng
        }



        doan5Entities database = new doan5Entities();
        public ActionResult AgreeCart()
        {
            // Lấy thông tin khách hàng từ session
            Customer khach = Session["TaiKhoan"] as Customer;
            if (khach != null)
            {
                List<CartItem> myCart = GetCart(); // Lấy giỏ hàng từ session
                OrderPro DonHang = new OrderPro(); // Tạo mới đơn đặt hàng
                DonHang.IDCus = khach.IDCus;
                DonHang.DateOrder = DateTime.Now;
                // Sử dụng địa chỉ giao hàng của khách hàng từ thông tin đăng ký tài khoản
                DonHang.AddressDelivery = khach.AddressCus;
                database.OrderProes.Add(DonHang);
                database.SaveChanges();
                // Lần lượt thêm từng chi tiết cho đơn hàng
                foreach (var product in myCart)
                {
                    OrderDetail chitiet = new OrderDetail();
                    chitiet.IDOrder = DonHang.ID;
                    chitiet.IDProduct = product.ProductID;
                    chitiet.Quantity = product.Number;
                    chitiet.UnitPrice = (decimal)(double)product.Price;
                    database.OrderDetails.Add(chitiet);
                }
                database.SaveChanges();
                // Xóa giỏ hàng sau khi đặt hàng thành công
                Session["GioHang"] = null;
                return RedirectToAction("Index", "CustomerProducts");
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin khách hàng trong session
                // Ví dụ: hiển thị thông báo lỗi và chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult ViewOrders()
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin khách hàng từ session
            Customer khach = Session["TaiKhoan"] as Customer;
            if (khach == null)
                return RedirectToAction("Login", "Users");

            // Lấy danh sách đơn hàng của khách hàng, bao gồm ghi chú
            var orders = database.OrderProes.Where(o => o.IDCus == khach.IDCus).ToList();

            return View(orders);
        }

        



        public ActionResult ViewOrderDetails(int orderId)
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin đơn hàng
            var order = database.OrderProes.FirstOrDefault(o => o.ID == orderId);
            if (order == null)
            {
                Console.WriteLine($"Không tìm thấy đơn hàng với ID: {orderId}");
                return RedirectToAction("ViewOrders");
            }

            // Lấy danh sách chi tiết đơn hàng
            var orderDetails = database.OrderDetails.Where(od => od.IDOrder == orderId).ToList();
            if (orderDetails == null || !orderDetails.Any())
            {
                Console.WriteLine($"Không có chi tiết đơn hàng cho Order ID: {orderId}");
            }

            ViewBag.Order = order;
            return View(orderDetails);
        }


        public ActionResult CancelOrder(int orderId)
        {
            var order = db.OrderProes.Find(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.StatusOrder == "Đã giao")
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng đã được xử lý!";
                return RedirectToAction("ViewOrders");
            }

            // Thực hiện hủy đơn hàng nếu trạng thái không phải "Đã xử lý"
            db.OrderProes.Remove(order);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công!";
            return RedirectToAction("ViewOrders");
        }







        private doan5Entities db = new doan5Entities();
        public ActionResult ApplyVoucher(int? productId, int voucherId)
        {
            // Lấy giỏ hàng từ Session
            List<CartItem> myCart = GetCart();

            // Tìm sản phẩm trong giỏ hàng
            CartItem product = myCart.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                // Xử lý khi không tìm thấy sản phẩm
                return RedirectToAction("GetCartInfo");
            }

            //  bảng Voucher trong cơ sở dữ liệu với các thông tin về mã giảm giá
            var voucher = db.Vouchers.FirstOrDefault(v => v.VoucherID == voucherId);
            if (voucher != null)
            {
                // Lưu ID của voucher và số tiền giảm giá vào sản phẩm tương ứng
                product.VoucherID = voucher.VoucherID;
                product.DiscountAmount = (int?)voucher.DiscountPercent;
            }

            // Lưu lại giỏ hàng đã cập nhật
            Session["GioHang"] = myCart;

            // Chuyển hướng về trang hiển thị thông tin giỏ hàng
            return RedirectToAction("GetCartInfo");
        }

        public ActionResult FailureView()
        {
            return View();
        }

        public ActionResult SuccessView()
        {
            return View();
        }


        //Thanh toán PAYPAL
        private void SaveOrderToDatabase(string orderNote)
        {
            var cartItems = GetCart();
            if (cartItems == null || !cartItems.Any())
                throw new Exception("Không có sản phẩm trong giỏ hàng.");

            var customer = (Customer)Session["TaiKhoan"];
            if (customer == null)
                throw new Exception("Khách hàng chưa đăng nhập.");

            var subtotal = CalculateSubtotal();

            using (var db = new doan5Entities())
            {
                var order = new OrderPro
                {
                    IDCus = customer.IDCus,
                    DateOrder = DateTime.Now,
                    AddressDelivery = customer.AddressCus,
                    Note = orderNote // Lưu ghi chú vào đơn hàng
                };

                db.OrderProes.Add(order);
                db.SaveChanges();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        IDOrder = order.ID,
                        IDProduct = item.ProductID,
                        Quantity = item.Number,
                        UnitPrice = (decimal)(double?)item.FinalPrice()
                    };
                    db.OrderDetails.Add(orderDetail);
                }

                db.SaveChanges();
            }
        }

        public decimal CalculateSubtotal()
        {
            List<CartItem> myCart = GetCart();
            if (myCart == null || !myCart.Any())
            {
                return 0;
            }

            // Tính tổng giá sau khi áp dụng giảm giá cho từng sản phẩm
            return myCart.Sum(item => item.FinalPrice());
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = $"{Request.Url.Scheme}://{Request.Url.Authority}/Cart/PaymentWithPaypal?";
                    string guid = Guid.NewGuid().ToString();

                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var approvalUrl = createdPayment.links
                        .FirstOrDefault(lnk => lnk.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

                    if (approvalUrl == null)
                        throw new Exception("Không tìm thấy URL phê duyệt từ PayPal.");

                    Session.Add(guid, createdPayment.id);
                    return Redirect(approvalUrl);
                }
                else
                {
                    string guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                        return View("FailureView");

                    // Lấy giá trị ghi chú từ form
                    string orderNote = Request.Form["orderNote"];

                    // Lưu thông tin đơn hàng vào cơ sở dữ liệu, bao gồm ghi chú
                    SaveOrderToDatabase(orderNote);

                    // Xóa giỏ hàng sau khi thanh toán
                    Session["GioHang"] = null;

                    var latestOrder = database.OrderProes.OrderByDescending(o => o.ID).FirstOrDefault();
                    if (latestOrder != null)
                    {
                        return RedirectToAction("ViewOrderDetails", new { orderId = latestOrder.ID });
                    }

                    return View("SuccessView");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi PayPal: {ex.Message}");
                return View("FailureView");
            }
        }




        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var cartItems = GetCart();
            if (cartItems == null || !cartItems.Any())
                throw new Exception("Giỏ hàng trống.");

            // Tạo danh sách các sản phẩm trong giỏ hàng
            var itemList = new ItemList
            {
                items = cartItems.Select(item => new Item
                {
                    name = item.NamePro,
                    currency = "USD",
                    price = (item.FinalPrice() / item.Number).ToString("0.00"),
                    quantity = item.Number.ToString(),
                    sku = item.ProductID.ToString()
                }).ToList()
            };

            var subtotal = CalculateSubtotal();

            var details = new Details
            {
                tax = "0",
                shipping = "0",
                subtotal = subtotal.ToString("0.00")
            };

            var amount = new Amount
            {
                currency = "USD",
                total = subtotal.ToString("0.00"),
                details = details
            };

            var transactionList = new List<Transaction>
            {
                new Transaction
                {
                    description = "Thanh toán đơn hàng",
                    invoice_number = Guid.NewGuid().ToString(),
                    amount = amount,
                    item_list = itemList
                }
            };

            var payer = new Payer { payment_method = "paypal" };

            var redirUrls = new RedirectUrls
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
     






        public ActionResult CancelSuccess()
        {
            return View();
        }

        [HttpPost]

        public ActionResult checkOut_Success(string orderNote)
        {
            // Lấy thông tin giỏ hàng (myCart)
            List<CartItem> cartItems = GetCart();

            // Lấy thông tin đơn hàng từ session hoặc từ cơ sở dữ liệu
            Customer customer = Session["TaiKhoan"] as Customer;
            if (customer == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Users");
            }

            // Tạo đơn hàng mới và lưu vào cơ sở dữ liệu
            OrderPro order = new OrderPro
            {
                IDCus = customer.IDCus,
                DateOrder = DateTime.Now,
                AddressDelivery = customer.AddressCus,
                Note = orderNote // Lưu ghi chú vào đơn hàng
            };

            db.OrderProes.Add(order);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
            foreach (var item in cartItems)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    IDOrder = order.ID, // Lấy ID đơn hàng vừa tạo
                    IDProduct = item.ProductID,
                    Quantity = item.Number,
                    UnitPrice = (decimal)(double?)item.Price,
                    SubTotal = item.FinalPrice(),
                    Note = orderNote // Lưu ghi chú vào chi tiết đơn hàng
                };

                db.OrderDetails.Add(orderDetail);
            }

            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["GioHang"] = null;

            return RedirectToAction("SuccessView"); // Hoặc trang nào bạn muốn hiển thị sau khi hoàn thành đơn hàng
        }





    }
}