using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Kiểu")]
        public string CouponType { get; set; }

        public enum ECouponType { Percent=0, Dollar=1}

        [Required]
        [Display(Name = "Số Tiền")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Số Tiền Tối Thiểu")]
        public double MinimumAmount { get; set; }

        [Display(Name = "Hình Ảnh")]
        public byte[] Picture { get; set; }

        [Display(Name = "Kích Hoạt?")]
        public bool IsActive { get; set; }
    }
}
