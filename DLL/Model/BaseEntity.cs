using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Model
{
    class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}
