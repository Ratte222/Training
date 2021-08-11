using BLL.Interfaces;
using DAL.EF;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity<long>
    {
        protected AppDBContext _context;
        public BaseService(AppDBContext appDBContext)
        {
            _context = appDBContext;
        }

        //public virtual void Create(T item)
        //{
        //    //I'll do it later through reflection 
        //    string temp = nameof(T);
        //    _context.Database.ExecuteSqlInterpolated($"INSERT INTO {temp}(Name, Descriprion, Cost) VALUES" +
        //        $"({})")
        //}

        //public virtual T Get(long id)
        //{
        //    string temp = nameof(T);
        //    _context.Set<T>().FromSqlInterpolated($"SELECT * FROM Products WHERE 'Id' = {id}");
        //}
    }
}
