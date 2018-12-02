using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class ExpConverter
    {
        public static int Convert(int exp)
        {
            return exp % 100;
        }
    }
}
