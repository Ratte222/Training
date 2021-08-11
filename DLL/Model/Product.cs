using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Model
{
    class Product : BaseEntity<long>
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int Cost { get; set; }
    }
}
