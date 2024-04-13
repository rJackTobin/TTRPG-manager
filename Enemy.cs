using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class Enemy : ICloneable, INotifyPropertyChanged, INameable
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

        public int MaxHP { get; set; }
        private int currentHP;

        public string ThreatLevel { get; set; }
        public string ImagePath { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public int CurrentHP
        {
            get { return currentHP; }
            set { currentHP = value; OnPropertyChanged(nameof(CurrentHP)); }
        }
    }
}
