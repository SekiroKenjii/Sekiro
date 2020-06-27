using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models.ViewModel
{
    public class OrderViewModel
    {
        public List<Order> Orders { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<DataPoint> DataPoints { get; set; }
    }
}
