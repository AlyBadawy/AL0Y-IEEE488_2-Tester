using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL0Y_IEEE488_2_Tester
{
    internal class HexParser
    {
        internal static string parse(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            var hexString = BitConverter.ToString(bytes);

            return hexString;
        }
    }
}
