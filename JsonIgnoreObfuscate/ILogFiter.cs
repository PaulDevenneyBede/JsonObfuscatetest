using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JsonIgnoreObfuscate
{
    public interface ILogFilter<T>
    {
        List<Expression<Func<T, object>>> PropertyFilters { get; }
        Dictionary<Expression<Func<T, object>>, Masker> PropertyObfuscaters { get; }

        ILogFilter<T> Ignore(Expression<Func<T, object>> func);
        ILogFilter<T> Obfuscate(Expression<Func<T, object>> func, Masker masker);
    }
}
