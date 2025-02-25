using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication3.Models;

namespace WebApplication3.Models
{
    public class CartItem
    {
        doan5Entities db = new doan5Entities();

        public int ProductID { get; set; }
        public string NamePro { get; set; }
        public string ImagePro { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
        // Thêm thuộc tính để lưu voucher áp dụng cho từng sản phẩm
        public int? VoucherID { get; set; }
        public int? DiscountAmount { get; set; } // Số tiền giảm giá từ voucher
                                                 // Thêm thuộc tính ghi chú
        public string Note { get; set; }


        // Constructor to initialize a CartItem from a ProductID
        public CartItem(int productID)
        {
            this.ProductID = productID;
            var productDB = db.Products.Single(s => s.ProductID == this.ProductID);
            this.NamePro = productDB.NamePro;
            this.ImagePro = productDB.ImagePro;
            this.Price = (decimal)productDB.Price;
            this.Number = 1;
        }

        // Method to calculate FinalPrice, considering any adjustments
        public decimal FinalPrice()
        {
            decimal finalPrice = Price * Number;

            // Trừ trực tiếp số tiền giảm giá
            if (DiscountAmount.HasValue && DiscountAmount.Value > 0)
            {
                finalPrice -= DiscountAmount.Value;
            }

            // Đảm bảo tổng giá không âm
            return finalPrice > 0 ? finalPrice : 0;
        }

    }
}
