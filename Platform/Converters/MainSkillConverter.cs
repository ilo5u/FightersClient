using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class MainSkillConverter
    {
        public static string Convert(int type)
        {
            switch (type)
            {
                case 1:
                    return "死者苏生：前期恢复血量，当达到上限时，不断提升攻击力";

                case 2:
                    return "天神下凡：强化所有属性增益，减少攻击间隔";

                case 3:
                    return "全副武装：大幅度提升自身防御力";

                case 4:
                    return "精神鼓舞：大幅度减少攻击间隔，将敌人的怒气值转化为自身攻击时附加的伤害";

                default:
                    return "";
            }
        }
    }
}
