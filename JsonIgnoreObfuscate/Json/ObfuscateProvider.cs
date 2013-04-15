using JsonIgnoreObfuscate.Maskers;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace JsonIgnoreObfuscate.Json
{
    public class ObfuscateProvider : IValueProvider
    {
        private readonly IMasker _masker;
        private readonly IValueProvider _underlyingValueProvider;

        public ObfuscateProvider(MemberInfo memberInfo, IMasker masker)
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
}
