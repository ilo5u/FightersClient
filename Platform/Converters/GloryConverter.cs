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
                return "初出茅庐 🌑";
            }
            else if (top < 5)
            {
                return "无名之辈 🌓";
            }
            else
            {
                return "斗宗强者 🌕";
            }
        }
    }
}
