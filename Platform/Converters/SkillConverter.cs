using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class SkillConverter
    {
        public static string Convert(int type, int skill)
        {
            switch (type)
            {
                case 1:
                    switch (skill)
                    {
                        case 0:
                            return "愤怒";

                        case 1:
                            return "自愈";

                        default:
                            return "";
                    }

                case 2:
                    switch (skill)
                    {
                        case 0:
                            return "致残";

                        case 1:
                            return "践踏";

                        default:
                            return "";
                    }

                case 3:
                    switch (skill)
                    {
                        case 0:
                            return "沉默";

                        case 1:
                            return "背刺";

                        default:
                            return "";
                    }

                case 4:
                    switch (skill)
                    {
                        case 0:
                            return "撕裂";

                        case 1:
                            return "减速";

                        default:
                            return "";
                    }

                default:
                    return "";
            }
        }
    }
}
