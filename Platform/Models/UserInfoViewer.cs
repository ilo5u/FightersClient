using Platform.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class UserInfoViewer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string name;
        private int numberOfPokemens;
        private string rate;
        private string honor;
        private string glory;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public int NumberOfPokemens
        {
            get
            {
                return numberOfPokemens;
            }
            set
            {
                numberOfPokemens = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NumberOfPokemens"));
                }
            }
        }

        public string Rate
        {
            get
            {
                return rate;
            }
            set
            {
                rate = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Rate"));
                }
            }
        }

        public string Honor
        {
            get
            {
                return honor;
            }
            set
            {
                honor = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Honor"));
                }
            }
        }

        public string Glory
        {
            get
            {
                return glory;
            }
            set
            {
                glory = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Glory"));
                }
            }
        }

        public void Renew(int total, int rounds, int wins, int top)
        {
            NumberOfPokemens = total;
            Rate = RateConverter.Convert(rounds, wins);
            Honor = HonorConverter.Convert(total);
            Glory = GloryConverter.Convert(top);
        }
    }
}
