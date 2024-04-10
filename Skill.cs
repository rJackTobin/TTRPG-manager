﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Skill : ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DamageType { get; set; }
        public int DamageAmount { get; set; }
        public int Cooldown { get; set; }
        public int BaseCooldown { get; set; }
        public int MPCost { get; set; }
        public int HPCost { get; set; }
        public int SkillLevel { get; set; }
        public int RemainingUses { get; set; }
        public int MaxUses { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public void UpSP(int amount)
        {
            this.Cooldown = Math.Min(this.BaseCooldown, this.Cooldown + amount);
        }
        public void DownSP(int amount)
        {
            this.Cooldown = Math.Max(0, this.Cooldown - amount);
        }
    }
}
