using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public static class characterManager
    {
        public static void AddItem(Character character, Item item)
        {
            if (character != null && item != null)
            {
                character.Inventory.Add(item);
            }
        }
        public static void NewItem(Character character)
        {
            if (character != null)
            {
                var newItem = new Item
                {
                    Name = "", // Blank name or some default value
                    Description = "", // Blank or default description
                    Uses = 0, // Default uses, adjust as necessary
                    MaxUses = 0, // Default max uses, adjust as necessary
                    Count = 1 // Assuming you're adding a single item at a time
                };

                // Step 4: Add the new item to the character's inventory
                character.Inventory.Add(newItem);
            }
        }
        public static void NewSkill(Character character)
        {
            if (character != null) {
                var newSkill = new Skill
                {
                    Name = "", // Blank name or some default value
                    Description = "", // Blank or default description
                    DamageType = "", // Default uses, adjust as necessary
                    MaxUses = 0 // Default max uses, adjust as necessary
                };

                // Step 4: Add the new item to the character's inventory
                character.Skills.Add(newSkill);
            }
        }
        public static void AddSkill(Character character, Skill skill)
        {
            if (character != null && skill != null)
            {
                character.Skills.Add(skill);
            }
        }
        public static void RemoveSkill(Character character, Skill skill)
        {
            if (character != null && skill != null)
            {
                character.Skills.Remove(skill);
            }
        }

    }
}
