using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class NotationOfCareerConverter
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
                            return "苦心研习治愈之术，拥有超高的血量以及更高的回血几率，在进入虚弱状态时，降低减益效果";

                        case 2:
                            return "为追求力量而荒废了治愈之术，但获得了更高的攻击力并且力能在短时内获得超高的爆发，如此同时，虚弱的后果会更加严重";

                        default:
                            return "";
                    }

                case 2:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "被授予战神称号的少年，挥舞着世上无人能举起的兵器，能轻松地摧毁敌人的武器，并力求一击将敌人制服，但行动却稍显迟缓";

                        case 2:
                            return "“既然不想受到伤害，就让敌人昏厥吧！”奉行这一准则的女神虽然抛弃了对于力量的追求，进而提升自己的行动速度以及令敌人眩晕的技巧，往往能在敌人毫无意识的时候将其击溃";

                        default:
                            return "";
                    }
                case 3:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "不断磨练自身的防御力，精心淬炼自身的盔甲，能比原来加倍奉还敌人的攻击";

                        case 2:
                            return "在别人的讥笑中发誓将摧毁他们灵魂的人，不断专研“沉默”技巧，当敌人遇上他时，将发现自身的技能无处施展，唯独战神不惧怕沉默的力量";

                        default:
                            return "";
                    }
                case 4:
                    switch (career)
                    {
                        case 0:
                            return "无";

                        case 1:
                            return "在夹缝中苟延残喘的人，来自那世界的阴暗面，堕入复仇的深渊，使用阴险奸诈的手段看着敌人死去，其极快的行动速度令人无法接触，在其狂暴时甚至能将敌人的愤怒化为自身的力量";

                        case 2:
                            return "在夹缝中苟延残喘的人，来自那世界的阴暗面，因不想像同类一样使用卑劣的手段获得胜利而不断锻炼自身的体术，强化防御力和攻击力，减缓敌人的行动速度";

                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }
}
