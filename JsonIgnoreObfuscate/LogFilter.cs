using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JsonIgnoreObfuscate
{
    public class LogFilter<T> : ILogFilter<T>
    {
        //list of the properties of a type to 
        public List<Expression<Func<T, object>>> PropertyFilters { get; internal set; }
        public Dictionary<Expression<Func<T, object>>, Masker> PropertyObfuscaters { get; internal set; }

        public LogFilter()
        {
            PropertyFilters = new List<Expression<Func<T, object>>>();
            PropertyObfuscaters = new Dictionary<Expression<Func<T, object>>, Masker>();
        }

        public ILogFilter<T> Ignore(Expression<Func<T, object>> func)
        {
            PropertyFilters.Add(func);
            return this;
        }

        public ILogFilter<T> Obfuscate(Expression<Func<T, object>> func, Masker masker)
        {
            //need to work out what to do with the mask
            PropertyObfuscaters.Add(func, masker);
            return this;
        }
    }
}
