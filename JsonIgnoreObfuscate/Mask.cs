using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonIgnoreObfuscate
{
    public class Masker
    {
        public int Size { get; set; }
        public char Padder { get; set; }
        public bool PadRight { get; set; }
        public string Pattern { get; set; }

        public string Mask(string target)
        {
            target = PadRight ? target.PadRight(Size, Padder) : target.PadLeft(Size, Padder);

            var builder = new StringBuilder();
            for (int i = 0; i < Pattern.Length; i++)
            {
                if (Pattern[i] == '#')
                {
                    builder.Append(target[i]);
                }
                else
                {
                    builder.Append(Pattern[i]);
                }    
            }

            return builder.ToString();
        }
    }
}
