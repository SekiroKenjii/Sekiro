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
        public string Name { get; set; }
        public string BrandImage { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Region { get; set; }
        public string Phone { get; set; }
        public string HomePage { get; set; }
    }
}
