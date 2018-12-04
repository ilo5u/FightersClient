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
                            return "愤怒：提高获得双倍怒气值的概率";

                        case 1:
                            return "自愈：提高回复血量的概率";

                        default:
                            return "";
                    }

                case 2:
                    switch (skill)
                    {
                        case 0:
                            return "致残：提高降低敌人攻击力的概率";

                        case 1:
                            return "践踏：提高致使敌人眩晕的概率";

                        default:
                            return "";
                    }

                case 3:
                    switch (skill)
                    {
                        case 0:
                            return "沉默：提高致使敌人沉默的概率";

                        case 1:
                            return "背刺：提高反弹伤害的概率";

                        default:
                            return "";
                    }

                case 4:
                    switch (skill)
                    {
                        case 0:
                            return "撕裂：提高致使敌人流血的概率";

                        case 1:
                            return "减速：提高致使敌人攻击间隔增大的概率";

                        default:
                            return "";
                    }

                default:
                    return "";
            }
        }
    }
}
