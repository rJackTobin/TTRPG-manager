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
    public class Character : ICloneable, INameable
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
        public bool actionTaken { get; set; }
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
        public void GiveItem(Item item)
        {
            this.Inventory.Add(item);
        }
        public void GiveSkill(Skill skill) 
        {
            this.Skills.Add(skill);
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

        public void useSkill(Skill skill)
        {
            for (int i = 0; i < this.Skills.Count; i++)
            {
                if (skill == this.Skills[i])
                {
                    this.Skills[i].Cooldown = 0;
                }
            }
        }
        
    }

}
