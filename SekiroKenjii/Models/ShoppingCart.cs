using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Range(1,int.MaxValue, ErrorMessage = "Hãy nhập ít nhất hoặc nhiều hơn {1} sản phẩm")]
        public int Count { get; set; }
    }
}
