﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SekiroKenjii.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Nhãn")]
        public string Name { get; set; }
    }
}
