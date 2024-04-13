using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Party : INameable
    {
        public ObservableCollection<Character> Members { get; set; }
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
        public int PartyLevel { get; set; } // Could be computed based on members' levels, if levels are implemented
        public int Currency { get; set; } // Shared party funds
        public Dictionary<string, int> SharedInventory { get; set; } // Shared items, e.g., potions, keys
        public ObservableCollection<StatusEffect> PartyStatusEffects { get; set; } // Effects that apply to the whole party

        public Party()
        {
            Members = new ObservableCollection<Character>();
            SharedInventory = new Dictionary<string, int>();
            PartyStatusEffects = new ObservableCollection<StatusEffect>();
        }

        // Additional functionality can be added here, such as methods to add/remove characters,
        // distribute loot, calculate average party level, etc.
    }
}
