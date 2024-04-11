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
        public string Name { get; set; }
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
