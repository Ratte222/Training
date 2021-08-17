using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DAL.Model;

namespace DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(AppDBContext db)
        {
            if(!db.OrderLines.Any())
            {
                OrderLine orderLine = new OrderLine()
                {
                    //OrderId = 1,
                    ProductId = 2,
                    Quantity = 5
                };
                OrderLine orderLine1 = new OrderLine()
                {
                    //OrderId = 2,
                    ProductId = 3,
                    Quantity = 14
                };
                OrderLine orderLine2 = new OrderLine()
                {
                    //OrderId = 3,
                    ProductId = 4,
                    Quantity = 50
                };
                db.OrderLines.AddRange(orderLine, orderLine1, orderLine2);
                db.SaveChanges();
            }
        }
    }
}
