using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Character
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string Title { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int MaxMP { get; set; }
        public int CurrentMP { get; set; }
        public List<Skill> Skills { get; set; }
        public List<string> Languages { get; set; }
        public List<Item> EquippedItems { get; set; }
        public List<Item> Inventory { get; set; }
        public int Currency { get; set; }
        public List<StatusEffect> StatusEffects { get; set; }
        public string ImagePath { get; set; }
        public int ActionsPerTurn { get; set; }

        public Character()
        {
            Skills = new List<Skill>();
            Languages = new List<string>();
            EquippedItems = new List<Item>();
            Inventory = new List<Item>();
            StatusEffects = new List<StatusEffect>();
        }

    }
}
