using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Nhà Cung Cấp")]
        public string Name { get; set; }

        [Display(Name = "Ảnh Thương Hiệu")]
        public string BrandImage { get; set; }

        [Display(Name = "Ảnh Thương Hiệu CLC")]
        public string HQBrandImage { get; set; }

        [Required]
        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Thành Phố")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Quốc Gia")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Vùng")]
        public string Region { get; set; }

        [Display(Name = "Số Điện Thoại")]
        public string Phone { get; set; }

        [Display(Name = "Trang Chủ")]
        public string HomePage { get; set; }
    }
}
