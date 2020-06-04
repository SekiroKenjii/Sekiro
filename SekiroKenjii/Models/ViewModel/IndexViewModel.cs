using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Coupon> Coupons { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
