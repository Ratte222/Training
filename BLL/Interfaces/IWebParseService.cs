using BLL.DTO.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IWebParseService
    {
        List<ProductParse> GetProduct();
    }
}
