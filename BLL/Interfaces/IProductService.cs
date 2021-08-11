using DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> Get(long id);
        void Create(Product item);
    }
}
