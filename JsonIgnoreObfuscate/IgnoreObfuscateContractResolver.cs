using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonIgnoreObfuscate
{
    public class ObfuscateProvider : IValueProvider
    {
        private readonly Masker _masker;
        private readonly IValueProvider _underlyingValueProvider;

        public ObfuscateProvider(MemberInfo memberInfo, Masker masker)
        {
            _underlyingValueProvider = new DynamicValueProvider(memberInfo);
            _masker = masker;
        }

        public object GetValue(object target)
        {
            return _masker.Mask(_underlyingValueProvider.GetValue(target).ToString());
        }

        public void SetValue(object target, object value)
        {
            _underlyingValueProvider.SetValue(target, value);
        }

    }


    public class IgnoreObfuscateContractResolver<T> : DefaultContractResolver
    {
        private readonly ILogFilter<T> _logfilter;

        //public new static readonly IgnoreObfuscateContractResolver<T> Instance = new IgnoreObfuscateContractResolver<T>(new LogFilter<T>());

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
