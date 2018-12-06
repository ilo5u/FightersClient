using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Platform.Converters
{
    class PokemenTypeConverter : IValueConverter
    {
        public static string ExternConvert(object value)
        {
            int type = (int)value;
            if (type == 1)
            {
                return "🦍";
            }
            else if (type == 2)
            {
                return "🦁";
            }
            else if (type == 3)
            {

                return "🦔";
            }
            else if (type == 4)
            {
                return "🦅";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static int ExternConvertBack(object value)
        {
            string type = (string)value;
            if (type == "🦍")
            {
                return 1;
            }
            else if (type == "🦁")
            {
                return 2;
            }
            else if (type == "🦔")
            {

                return 3;
            }
            else if (type == "🦅")
            {
                return 4;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int type = (int)value;
            if (type == 1)
            {
                return "🦍";
            }
            else if (type == 2)
            {
                return "🦁";
            }
            else if (type == 3)
            {

                return "🦔";
            }
            else if (type == 4)
            {
                return "🦅";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
