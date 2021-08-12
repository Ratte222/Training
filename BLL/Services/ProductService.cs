using BLL.DTO.Product;
using BLL.Extensions;
using BLL.Filters;
using BLL.Helpers;
using BLL.Infrastructure;
using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BLL.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly AppSettings _appSettings;
        public ProductService(AppDBContext appDBContext, IOptions<AppSettings> options): base(appDBContext)
        {
            _appSettings = options.Value;
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

        public void Serialize(ProductDTO product)
        {
            string _path = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.DirectoryForSerializeDeserialize);
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            _path = Path.Combine(_path, _appSettings.FileName);
            #region Binary
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // To serialize the hashtable and its key/value pairs,
            // you must first open a stream for writing.
            // In this case, use a file stream.
            using (FileStream fs = new FileStream(_path + ".dat", FileMode.OpenOrCreate))
            {
                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, product);
            }
            #endregion
            #region xml
            
            using (FileStream fs = new FileStream(_path + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(ProductDTO));
                xmlFormatter.Serialize(fs, product);
            }
            #endregion
            #region json
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(_path+".json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, product);                
            }

            #endregion
        }

        public List<ProductDTO> Deserialize()
        {
            List<ProductDTO> productDTOs = new List<ProductDTO>();
            string _path = Path.Combine(Directory.GetCurrentDirectory(), 
                _appSettings.DirectoryForSerializeDeserialize, _appSettings.FileName);

            #region binary
            using (FileStream fs = new FileStream(_path + ".dat", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and
                // assign the reference to the local variable.
                productDTOs.Add((ProductDTO)formatter.Deserialize(fs));
            }                
            #endregion

            #region xml
            XmlSerializer xmlFrmatter = new XmlSerializer(typeof(ProductDTO));            
            using (FileStream fs = new FileStream(_path + ".xml", FileMode.Open))
            {
                productDTOs.Add((ProductDTO)xmlFrmatter.Deserialize(fs));
            }
            #endregion
            #region Json
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamReader sr = new StreamReader(_path + ".json"))
            using (JsonReader writer = new JsonTextReader(sr))
            {
                productDTOs.Add((ProductDTO)serializer.Deserialize(sr, typeof(ProductDTO)));
            }
            #endregion

            return productDTOs;
        }
    }
}
