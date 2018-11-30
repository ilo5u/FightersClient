using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class ExpConverter
    {
        public static int Convert(int exp, int level)
        {
            return level == 0 ? exp : exp % ((level - 1) * 1000);
        }
    }
}
