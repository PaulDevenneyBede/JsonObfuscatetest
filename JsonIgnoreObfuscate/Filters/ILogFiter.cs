using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JsonIgnoreObfuscate.Maskers;

namespace JsonIgnoreObfuscate.Filters
{
    public interface ILogFilter<T>
    {
        List<Expression<Func<T, object>>> PropertyFilters { get; }
        Dictionary<Expression<Func<T, object>>, IMasker> PropertyObfuscaters { get; }

        ILogFilter<T> Ignore(Expression<Func<T, object>> func);
        ILogFilter<T> Obfuscate(Expression<Func<T, object>> func, IMasker masker);
    }
}
