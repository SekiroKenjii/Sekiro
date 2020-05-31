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
        public string Name { get; set; }

        public string Image { get; set; }

        public string HQImage { get; set; }

        public string SortName { get; set; }

        [Display(Name ="Supplier")]
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name="Tag")]
        public int? TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        [Range(1,int.MaxValue, ErrorMessage = "Price should be greater than ${1}")]
        public double Price { get; set; }

        [Display(Name ="Units In Stock")]
        public int UnitsInStock { get; set; }

        [Display(Name ="Units On Oder")]
        public int UnitsOnOder { get; set; }

        [DefaultValue(false)]
        public bool Discontinued { get; set; } = false;

        public string Description { get; set; }
    }
}
