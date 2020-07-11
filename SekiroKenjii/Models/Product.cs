using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; }

        [Display(Name = "Hình Ảnh CLT")]
        public string Image { get; set; }

        [Display(Name = "Hình Ảnh CLC")]
        public string HQImage { get; set; }

        [Display(Name = "Tên Rút Gọn")]
        public string SortName { get; set; }

        [Display(Name ="Mã Nhà Cung Cấp")]
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [Display(Name ="Mã Loại")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name="Mã Nhãn")]
        public int? TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        [Display(Name = "Giá")]
        [Range(1,int.MaxValue, ErrorMessage = "Price should be greater than {1}")]
        public double Price { get; set; }

        [Display(Name ="Số Lượng Trong Kho")]
        public int UnitsInStock { get; set; }

        [Display(Name ="Số Lượng Trên Đơn Hàng")]
        public int UnitsOnOder { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Ngừng Kinh Doanh?")]
        public bool Discontinued { get; set; } = false;

        [Display(Name = "Chi Tiết")]
        public string Description { get; set; }
    }
}
