using Platform.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class PokemenViewer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int id;
        private int type;
        private string image;
        private string name;
        private int hpoints;
        private int attack;
        private int defense;
        private int agility;
        private int interval;
        private int critical;
        private int hitratio;
        private int parryratio;
        private int career;
        private int level;
        private int exp;
        private int primarySkill;
        private int secondSkill;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }

        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Image"));
                }
            }
        }

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

        public int Hpoints
        {
            get
            {
                return hpoints;
            }
            set
            {
                hpoints = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Hpoints"));
                }
            }
        }

        public int Attack
        {
            get
            {
                return attack;
            }
            set
            {
                attack = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Attack"));
                }
            }
        }

        public int Defense
        {
            get
            {
                return defense;
            }
            set
            {
                defense = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Defense"));
                }
            }
        }

        public int Agility
        {
            get
            {
                return agility;
            }
            set
            {
                agility = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Agility"));
                }
            }
        }

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Interval"));
                }
            }
        }

        public int Critical
        {
            get
            {
                return critical;
            }
            set
            {
                critical = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Critical"));
                }
            }
        }

        public int Hitratio
        {
            get
            {
                return hitratio;
            }
            set
            {
                hitratio = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Hitratio"));
                }
            }
        }

        public int Parryratio
        {
            get
            {
                return parryratio;
            }
            set
            {
                parryratio = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Parryratio"));
                }
            }
        }

        public int Career
        {
            get
            {
                return career;
            }
            set
            {
                career = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Career"));
                }
            }
        }

        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Level"));
                }
            }
        }

        public int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                exp = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Exp"));
                }
            }
        }

        public int PrimarySkill
        {
            get
            {
                return primarySkill;
            }
            set
            {
                primarySkill = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PrimarySkill"));
                }
            }
        }
        public int SecondSkill
        {
            get
            {
                return secondSkill;
            }
            set
            {
                secondSkill = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SecondSkill"));
                }
            }
        }

        public static explicit operator PokemenViewer(Kernel.Property property)
        {
            string imagePath = ImageConverter.Convert(property.type, property.career);
            PokemenViewer viewer = new PokemenViewer
            {
                Id = property.id,

                Type = property.type,
                Image = imagePath,
                Name = property.name,

                Hpoints = property.hpoints,
                Attack = property.attack,
                Defense = property.defense,
                Agility = property.agility,

                Interval = property.interval,
                Critical = property.critical,
                Hitratio = property.hitratio,
                Parryratio = property.parryratio,

                Career = property.career,
                Exp = property.exp,
                Level = property.level,

                PrimarySkill = property.primarySkill,
                SecondSkill = property.secondSkill
            };
            return viewer;
        }

        public void Renew(int type, 
            int hpoints, int attack, int defense, int agility,
            int interval, int critical, int hitratio, int parryratio,
            int career, int exp, int level)
        {
            Type = type;
            Image = ImageConverter.Convert(type, career);
            Hpoints = hpoints;
            Attack = attack;
            Defense = defense;
            Agility = agility;
            Interval = interval;
            Critical = critical;
            Hitratio = hitratio;
            Parryratio = parryratio;
            Career = career;
            Exp = exp;
            Level = level;
        }
    }
}
