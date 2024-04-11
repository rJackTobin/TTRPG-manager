using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Enemy : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }

        public string ThreatLevel { get; set; }
        public string ImagePath { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
