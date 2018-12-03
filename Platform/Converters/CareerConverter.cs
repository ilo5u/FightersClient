using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class CareerConverter
    {
        public static string Convert(int type, int career)
        {
            switch (type)
            {
                case 1:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "光明大法师";

                        case 2:
                            return "黑暗大法师";

                        default:
                            return "";
                    }

                case 2:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "战神·阿瑞斯";

                        case 2:
                            return "智慧女神·雅典娜";

                        default:
                            return "";
                    }
                case 3:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "守护者·帕拉丁";

                        case 2:
                            return "异面行者·小丑";

                        default:
                            return "";
                    }
                case 4:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "深渊猎手·尤迪安";

                        case 2:
                            return "审判者·米歇尔";

                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }
}
