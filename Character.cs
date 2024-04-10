using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Character : ICloneable
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
        public ObservableCollection<Skill> Skills { get; set; }
        public ObservableCollection<string> Languages { get; set; }
        public ObservableCollection<Item> EquippedItems { get; set; }
        public ObservableCollection<Item> Inventory { get; set; }
        public int Currency { get; set; }
        public ObservableCollection<StatusEffect> StatusEffects { get; set; }
        public string ImagePath { get; set; }
        public int ActionsPerTurn { get; set; }
        public double eyeLevel { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Character()
        {
            Skills = new ObservableCollection<Skill>();
            Languages = new ObservableCollection<string>();
            EquippedItems = new ObservableCollection<Item>();
            Inventory = new ObservableCollection<Item>();
            StatusEffects = new ObservableCollection<StatusEffect>();
        }
        public void RemoveItem(Item item)
        {
            if (!this.Inventory.Remove(item))
            {
                this.EquippedItems.Remove(item);
            }
        }
        public void Equip(Item item)
        {
            this.EquippedItems.Add(item);
            this.Inventory.Remove(item);
        }
        public void Unequip(Item item)
        {
            this.Inventory.Add(item);
            this.EquippedItems.Remove(item);
        }
        public void RemoveSkill(Skill skill)
        {
            this.Skills.Remove(skill);
        }
        public void UpHP(int amount)
        {
            this.CurrentHP = Math.Min(this.MaxHP, this.CurrentHP + amount);
        }
        public void DownHP(int amount)
        {
            this.CurrentHP = Math.Max(0, this.CurrentHP - amount);
        }
        public void UpMP(int amount)
        {
            this.CurrentMP = Math.Min(this.MaxMP, this.CurrentMP + amount);
        }
        public void DownMP(int amount)
        {
            this.CurrentMP = Math.Max(0, this.CurrentMP - amount);
        }
    }

}
