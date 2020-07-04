using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mã Đơn Hàng")]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        [Display(Name = "Mã Sản Phẩm")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int Count { get; set; }

        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; }

        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Giá 1 Sản Phẩm")]
        public double Price { get; set; }
    }
}
