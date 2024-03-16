﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Party
    {
        public ObservableCollection<Character> Members { get; set; }
        public string Name { get; set; }
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
