using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class GloryConverter
    {
        static public string Convert(int top)
        {
            if (top < 1)
            {
                return "🌑";
            }
            else if (top < 5)
            {
                return "🌓";
            }
            else
            {
                return "🌕";
            }
        }
    }
}
