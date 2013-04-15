using JsonIgnoreObfuscate.Filters;
using JsonIgnoreObfuscate.Json;
using JsonIgnoreObfuscate.Maskers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonIgnoreObfuscate
{
    class Program
    {
        private static readonly Dictionary<Type, object> registeredFilters = new Dictionary<Type, object>();

        static void Main(string[] args)
        {
            IMasker creditCardMasker = new DefaultMasker(){ Pattern = "************####", Size = 16};
            IMasker passwordMasker = new DefaultMasker() { Pattern = "****************", Size = 16 };

            var cc1 = creditCardMasker.Mask("1234567890123456");
            var cc2 = creditCardMasker.Mask("123456789012345");
            
            var test = new MyTestClass()
                {
                    CardNumber = "1234567890123456",
                    Password = "verysecret",
                    Description = "way too long"
                };

            ILogFilter<MyTestClass> filter =
                new LogFilter<MyTestClass>()
                    .Ignore(f => f.Description)
                    .Obfuscate(f => f.Password, passwordMasker)
                    .Obfuscate(f => f.CardNumber, creditCardMasker);

            ILogFilter<MyTestClass2> filter2 =
                new LogFilter<MyTestClass2>()
                    .Ignore(f => f.Description)
                    .Obfuscate(f => f.Password, passwordMasker)
                    .Obfuscate(f => f.CardNumber, creditCardMasker);

            RegisterFilter(filter);
            RegisterFilter(filter2);

            var resolver = new IgnoreObfuscateContractResolver<MyTestClass>(filter);
            var settings = new JsonSerializerSettings {ContractResolver = resolver};
            var result = JsonConvert.SerializeObject(test, settings);

            Console.ReadKey();
        }

        private static void RegisterFilter<T>(ILogFilter<T> filter)
        {
            registeredFilters.Add(typeof (T), filter);
        }
    }
}



