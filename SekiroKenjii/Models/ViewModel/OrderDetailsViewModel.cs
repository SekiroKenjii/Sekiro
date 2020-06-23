using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models.ViewModel
{
    public class OrderDetailsViewModel
    {
        public Order Orders { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
