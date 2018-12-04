using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Converters
{
    class ImageConverter
    {
        static public string Convert(int type, int career)
        {
            switch (type)
            {
                case 1:
                    switch (career)
                    {
                        case 0:
                            return "/Pictures/Master.png";

                        case 1:
                            return "/Pictures/Lighter.png";

                        case 2:
                            return "/Pictures/Darker.png";

                        default:
                            return "";
                    }

                case 2:
                    switch (career)
                    {
                        case 0:
                            return "/Pictures/Knight.png";

                        case 1:
                            return "/Pictures/Ares.png";

                        case 2:
                            return "/Pictures/Athena.png";

                        default:
                            return "";
                    }
                case 3:
                    switch (career)
                    {
                        case 0:
                            return "/Pictures/Guardian.png";

                        case 1:
                            return "/Pictures/Paladin.png";

                        case 2:
                            return "/Pictures/Joker.png";

                        default:
                            return "";
                    }
                case 4:
                    switch (career)
                    {
                        case 0:
                            return "/Pictures/Assassin.png";

                        case 1:
                            return "/Pictures/Yodian.png";

                        case 2:
                            return "/Pictures/Michelle.png";

                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }
}
