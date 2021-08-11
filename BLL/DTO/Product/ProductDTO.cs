using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Product
{
    public class ProductDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
        public double Cost { get; set; }
    }
}
