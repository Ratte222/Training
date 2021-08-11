using BLL.DTO.Product;
using BLL.Extensions;
using BLL.Filters;
using BLL.Helpers;
using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(AppDBContext appDBContext): base(appDBContext)
        {

        }

        public virtual Product Get(long id)
        {
            Product item = new Product();
            string tableName = nameof(Product) + 's';
            string query = $"SELECT * FROM {tableName} WHERE {nameof(item.Id)} = '{id}'";
            return _context.Set<Product>().FromSqlRaw(query)
                .FirstOrDefault();
        }

        public virtual IQueryable<Product> GetAll()
        {
            string tableName = nameof(Product) + 's';
            string query = $"SELECT * FROM {tableName}";
            return _context.Set<Product>().FromSqlRaw(query);
        }

        public void Create(Product item)
        {
            string query = $"INSERT INTO {nameof(Product)}s ({nameof(item.Name)}, {nameof(item.Description)}," +
                $" {nameof(item.Cost)}) VALUES ('{item.Name}'," +
                $" '{item.Description}', '{item.Cost}')";           
            _context.Database.ExecuteSqlRaw(query);
        }

        public void Update(Product item)
        {
            string query = $"UPDATE {nameof(Product)}s SET {nameof(item.Name)} = '{item.Name}', " +
                $"{nameof(item.Description)} = '{item.Description}', {nameof(item.Cost)} = '{item.Cost}' " +
                $"WHERE {nameof(item.Id)} = '{item.Id}'";
            _context.Database.ExecuteSqlRaw(query);
        }
            
        public void Delete(long id)
        {
            Product item = new Product();
            string query = $"DELETE FROM {nameof(Product)}s " +
                $"WHERE {nameof(item.Id)} = '{id}'";
            _context.Database.ExecuteSqlRaw(query);
        }

        public IEnumerable<Product> GetProducts(PageResponse<ProductDTO> pageResponse, ProductFilter productFilter)
        {
            var query = GetAll();
            if (String.IsNullOrEmpty(productFilter.FieldOrderBy))
            {
                Product product = new Product();
                query = query.OrderBy(nameof(product.Name), productFilter.OrderByDescending);
            }
            else
                query = query.OrderBy(productFilter.FieldOrderBy, productFilter.OrderByDescending);
            return query.Skip(pageResponse.Skip).Take(pageResponse.Take);
        }

        
    }
}
