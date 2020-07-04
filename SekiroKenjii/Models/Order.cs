using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Ngày Đặt Hàng")]
        public DateTime OrderDate { get; set; }

        [Required]
        [NotMapped]
        [Display(Name = "Thời Gian Đặt Hàng")]
        public DateTime OrderTime { get; set; }

        [Required]
        [Display(Name = "Tổng Tiền Đặt Hàng Gốc")]
        public double OrderTotalOriginal { get; set; }

        [Required]
        [Display(Name = "Tổng Tiền Đặt Hàng")]
        public double OrderTotal { get; set; }

        [Display(Name = "Mã Giãm Giá")]
        public string CouponCode { get; set; }

        [Display(Name = "Số Tiền Giãm")]
        public double CouponCodeDiscount { get; set; }

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; }

        [Display(Name = "Trạng Thái Thanh Toán")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Bình Luận")]
        public string Comment { get; set; }

        [Display(Name = "Tên Khách Hàng")]
        public string OrderName { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Mã Giao Dịch")]
        public string TransactionId { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string ShipAddress { get; set; }

        [Display(Name = "Thành Phố")]
        public string ShipCity { get; set; }

        [Display(Name = "Quốc Gia")]
        public string ShipCountry { get; set; }

        [Display(Name = "Email")]
        public string ShipEmail { get; set; }
    }
}
