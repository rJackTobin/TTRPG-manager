using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Skill : ICloneable, INameable
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                safeName = MakeSafeName(_name);
            }
        }
        public string safeName { get; set; }
        private string MakeSafeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            // Remove all non-alphanumeric characters
            return System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9]", "");
        }
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
