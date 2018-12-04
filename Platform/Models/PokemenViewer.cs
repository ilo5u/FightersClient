using Platform.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class PokemenViewer
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Hpoints { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Agility { get; set; }
        public int Interval { get; set; }
        public int Critical { get; set; }
        public int Hitratio { get; set; }
        public int Parryratio { get; set; }
        public int Career { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int PrimarySkill { get; set; }
        public int SecondSkill { get; set; }

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

        public void Renew(int type, string image, 
            int hpoints, int attack, int defense, int agility,
            int interval, int critical, int hitratio, int parryratio,
            int career, int exp, int level)
        {
            Type = type;
            Image = image;
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
