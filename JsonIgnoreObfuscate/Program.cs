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
        static void Main(string[] args)
        {
            var creditCardMasker = new Masker(){ Pattern = "************####", Size = 16};
            var passwordMasker = new Masker() { Pattern = "****************", Size = 16 };

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

            Add(filter);
            Add(filter2);

            var resolver = new IgnoreObfuscateContractResolver<MyTestClass>(filter);
            var settings = new JsonSerializerSettings {ContractResolver = resolver};
            var result = JsonConvert.SerializeObject(test, settings);

            Console.ReadKey();
        }

        private static void Add<T>(ILogFilter<T> filter)
        {
            var coll = new Dictionary<Type, object>();
            coll.Add(typeof (T), filter);
        }
    }
}



