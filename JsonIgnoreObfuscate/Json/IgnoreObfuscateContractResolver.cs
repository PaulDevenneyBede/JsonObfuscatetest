using JsonIgnoreObfuscate.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JsonIgnoreObfuscate.Json
{
    public class IgnoreObfuscateContractResolver<T> : DefaultContractResolver
    {
        private readonly ILogFilter<T> _logfilter;

        public IgnoreObfuscateContractResolver(ILogFilter<T> logfilter)
        {
            _logfilter = logfilter;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
         {
           var property = base.CreateProperty(member, memberSerialization);

           property.ShouldSerialize = instance => !_logfilter.PropertyFilters.Select(GetPropertyName).Contains(property.PropertyName);

           if (_logfilter.PropertyObfuscaters.Select(o => GetPropertyName(o.Key)).Contains(property.PropertyName))
           {
               var mask = _logfilter.PropertyObfuscaters.First(o => GetPropertyName(o.Key) == property.PropertyName).Value;
               property.ValueProvider = new ObfuscateProvider(member, mask);
            }
            
           return property;
         }

        private string GetPropertyName(Expression<Func<T, object>> propertyRefExpr)
        {
            if (propertyRefExpr == null)
                throw new ArgumentNullException("propertyRefExpr", "propertyRefExpr is null.");

            var memberExpr = propertyRefExpr.Body as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = propertyRefExpr.Body as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException("No property reference expression was found.",
                             "propertyRefExpr");
        }
    }
}
