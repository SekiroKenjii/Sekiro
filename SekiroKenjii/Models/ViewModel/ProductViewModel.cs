using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models.ViewModel
{
    public class ProductViewModel
    {
        public Product Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
