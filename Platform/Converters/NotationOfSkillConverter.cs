using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class NotationOfSkillConverter
    {
        public static string Convert(int type)
        {
            switch (type)
            {
                case 1:
                    return "愤怒：主动技能，在下次被攻击时获得双倍的怒气点加成\n自愈：主动技能，回复自身小部分血量";

                case 2:
                    return "致残：主动技能，摧毁敌人武器，迫使敌人减少攻击力\n践踏：主动技能，眩晕敌人，使其无法攻击";

                case 3:
                    return "沉默：主动技能，攻击敌人的灵魂，使敌人无法使用技能\n背刺：被动技能，身披覆盖利刺的盔甲，在受到攻击时，向敌人返还伤害";

                case 4:
                    return "撕裂：主动技能，利刃划破敌人的躯体，使敌人不断失血而受到伤害\n减速：主动技能，削弱敌人的行动力，延长敌人的攻击间隔";

                default:
                    return "";
            }
        }
    }
}
