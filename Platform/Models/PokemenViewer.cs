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

        public static explicit operator PokemenViewer(Kernel.Property property)
        {
            PokemenViewer viewer = new PokemenViewer
            {
                Id = property.id,

                Type = property.type,
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
                Level = property.level
            };
            return viewer;
        }
    }
}
