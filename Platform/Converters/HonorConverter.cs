using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class HonorConverter
    {
        static public string Convert(int total)
        {
            if (total < 5)
            {
                return "🥉";
            }
            else if (total < 20)
            {
                return "🥈";
            }
            else
            {
                return "🥇";
            }
        }
    }
}
