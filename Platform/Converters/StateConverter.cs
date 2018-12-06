using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Platform.Converters
{
    public class StateConverter : IValueConverter
    {
        public enum StateType
        {
            ANGRIED = 0x0001,

            BLEED = 0x0004,
            SLOWED = 0x0008,
            WEAKEN = 0x0010,
            SUNDERED = 0x0020,
            DIZZYING = 0x0040,
            SILENT = 0x0080,

            RAGED = 0x0100,
            REBOUND = 0x0200,
            AVATAR = 0x0400,
            INSPIRED = 0x0800,
            ARMOR = 0x1000
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            StateType type = (StateType)value;
            switch (type)
            {
                case StateType.ANGRIED:
                    return "👿";

                case StateType.BLEED:
                    return "💉";

                case StateType.SLOWED:
                    return "🚫";

                case StateType.WEAKEN:
                    return "😨";

                case StateType.SUNDERED:
                    return "🛠";

                case StateType.DIZZYING:
                    return "💫";

                case StateType.SILENT:
                    return "💬";

                case StateType.RAGED:
                    return "🤬";

                case StateType.REBOUND:
                    return "🌟";

                case StateType.AVATAR:
                    return "💪";

                case StateType.INSPIRED:
                    return "📯";

                case StateType.ARMOR:
                    return "🛡";

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
