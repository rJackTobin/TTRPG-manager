using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Item : INameable, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Uses { get; set; }
        public int MaxUses { get; set; }
        public int Count { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
