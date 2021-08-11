using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(AppDBContext appDBContext): base(appDBContext)
        {

        }

        public void Create(Product item)
        {
            //I'll do it later through reflection 
            string temp = nameof(Product);
            _context.Database.ExecuteSqlInterpolated($"INSERT INTO {temp}(Name, Descriprion, Cost) VALUES ({item.Id}, {item.Description}, {item.Cost})");
        }

        public async Task<Product> Get(long id)
        {
            string temp = nameof(Product);
            return await _context.Set<Product>().FromSqlInterpolated($"SELECT * FROM Products WHERE 'Id' = {id}")
                .FirstOrDefaultAsync();
        }
    }
}
