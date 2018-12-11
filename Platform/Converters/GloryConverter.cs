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
            if (top < 2)
            {
                return "无名之辈";
            }
            else if (top == 3)
            {
                return "初出茅庐";
            }
            else if (top == 4)
            {
                return "登堂入室 🌑";
            }
            else if (top == 5)
            {
                return "绝代宗师 🌓";
            }
            else
            {
                return "破碎虚空 🌕";
            }
        }
    }
}
