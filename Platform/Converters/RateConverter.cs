using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class RateConverter
    {
        static public string Convert(int rounds, int wins)
        {
            if (rounds == 0)
                return (0.00).ToString() + "%";
            else
                return (((double)wins / (double)rounds) * 100.0).ToString() + "%";
        }
    }
}
