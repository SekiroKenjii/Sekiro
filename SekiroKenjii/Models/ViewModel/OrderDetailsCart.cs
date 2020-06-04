using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models.ViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> lstCarts { get; set; }
        public Order Orders { get; set; }

    }
}
