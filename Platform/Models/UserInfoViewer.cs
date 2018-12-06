using Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class UserInfoViewer
    {
        public string Name { get; set; }
        public int NumberOfPokemens { get; set; }
        public string Rate { get; set; }
        public string Honor { get; set; }
        public string Glory { get; set; }

        public void Renew(int total, int rounds, int wins, int top)
        {
            NumberOfPokemens = total;
            Rate = RateConverter.Convert(rounds, wins);
            Honor = HonorConverter.Convert(total);
            Glory = GloryConverter.Convert(top);
        }
    }
}
