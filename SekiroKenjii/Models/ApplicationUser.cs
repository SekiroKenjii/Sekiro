using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string FullName { get; set; }
    }
}
