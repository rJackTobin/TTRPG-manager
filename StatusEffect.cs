using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class StatusEffect : INameable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBuff { get; set; }
        public bool IsDebuff { get; set; }
        public string Condition { get; set; }
    }
}
