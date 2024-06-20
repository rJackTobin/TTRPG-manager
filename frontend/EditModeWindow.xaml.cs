﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;




namespace TTRPG_manager
{
    /// <summary>
    /// Interaction logic for EditModeWindow.xaml
    /// </summary>
    /// 

    public partial class EditModeWindow : Window
    {
        private AppConfig updated_config;
        // This property will be used for the binding of characterComboBox's ItemsSource
        public ObservableCollection<Character> MembersComboBoxItemsSource => updated_config.Parties[partyComboBox.SelectedIndex].Members;

        public EditModeWindow(double height, double width)
        {
            InitializeComponent();

            updated_config = ConfigManager.LoadConfig();
            // Or however you instantiate it

            // Copy values from old_config to new_config if needed

            this.DataContext = updated_config; // Set DataContext for data binding


            this.Width = width*0.8;
            this.Height = height*0.8;
            this.Closing += Window_Closing;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
        public void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            int i = partyComboBox.SelectedIndex;
            if (i != -1)
            {
                Character newChar = new Character();
                newChar.Name = "New Character";
                updated_config.Parties[i].Members.Add(newChar);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void AddCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            int i = partyComboBox.SelectedIndex;
            int j = CharacterComboBox.SelectedIndex;
            if (i != -1 && j != -1)
            {
                updated_config.Parties[i].Members.Add(updated_config.Characters[j]);
                updated_config.Characters.RemoveAt(j);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void CreateCharacterButton_Click(object sender, RoutedEventArgs e)
        {
                Character newChar = new Character();
                newChar.Name = "Unassigned Character";
                CharacterEditWindow newCharEdit = new CharacterEditWindow(Height, Width, newChar);
                var dialogResult = newCharEdit.ShowDialog();
                if (dialogResult == true)
                {
                    updated_config.Characters.Add(newChar);
                    ConfigManager.SaveConfig(updated_config);
                }
        }
        public void DeleteCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            int i = CharacterComboBox.SelectedIndex;
            if (i != -1)
            {
                updated_config.Characters.RemoveAt(i);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void CreatePartyButton_Click(object sender, RoutedEventArgs e)
        {
            Party newParty = new Party();
            newParty.Name = partyNameTextBox.Text;
            updated_config.Parties.Add(newParty);
            partyNameTextBox.Text = "";
            ConfigManager.SaveConfig(updated_config);
        }
        public void DeletePartyButton_Click(object sender, RoutedEventArgs e)
        {
            if (partyComboBox.SelectedIndex != -1)
            {
                int i = partyComboBox.SelectedIndex;
                updated_config.Parties.Remove(updated_config.Parties[i]);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void CreateItemButton_Click(object sender, RoutedEventArgs e)
        {
            Item newItem = new Item();
            
            newItem.Name = ItemNameBox.Text;
            newItem.Description = ItemDescBox.Text;
            newItem.Count = 1;
            if (newItem.Name == "")
            { return; }
            updated_config.Items.Add(newItem);
            ConfigManager.SaveConfig(updated_config);
            ItemNameBox.Text = null; ItemDescBox.Text = null;
        }
        public void SaveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                updated_config.Items[ItemList.SelectedIndex].Name = ItemNameBox.Text;
                updated_config.Items[ItemList.SelectedIndex].Description = ItemDescBox.Text;
                ConfigManager.SaveConfig(updated_config);
                ItemNameBox.Text = null; ItemDescBox.Text = null;
            }
        }

        public void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                updated_config.Items.RemoveAt(ItemList.SelectedIndex);
                ConfigManager.SaveConfig(updated_config);
                ItemNameBox.Text = null; ItemDescBox.Text = null;
            }
        }

        private void PartyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the selected index is valid
            if (partyComboBox.SelectedIndex != -1)
            {
                // Directly set the ItemsSource of the character ComboBox
                memberComboBox.ItemsSource = updated_config.Parties[partyComboBox.SelectedIndex].Members;

                // If using DisplayMemberPath to show character names, ensure it's set (assuming Character class has a Name property)
                memberComboBox.DisplayMemberPath = "Name";
            }
            else
            {
                // Clear the character ComboBox if no party is selected
                memberComboBox.ItemsSource = null;
            }
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemList.SelectedIndex != -1)
            {
                ItemNameBox.Text = updated_config.Items[ItemList.SelectedIndex].Name;
                ItemDescBox.Text = updated_config.Items[ItemList.SelectedIndex].Description;
            }
        }
        

        private void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            int i = partyComboBox.SelectedIndex;
            int j = memberComboBox.SelectedIndex;
            if (i != -1 && j != -1)
            {
                updated_config.Characters.Add(updated_config.Parties[i].Members[j]);
                updated_config.Parties[i].Members.RemoveAt(j);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void CreateSkillButton_Click(object sender, RoutedEventArgs e)
        {
            Skill newSkill = new Skill();
            newSkill.Name = "New Skill";
            SkillEditWindow newCharEdit = new SkillEditWindow(Height, Width, newSkill);
            var dialogResult = newCharEdit.ShowDialog();
            if (dialogResult == true)
            {
                updated_config.Skills.Add(newSkill);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void DeleteSkillButton_Click(object sender, RoutedEventArgs e)
        {
            int i = SkillList.SelectedIndex;
            if (i != -1)
            {
                updated_config.Skills.RemoveAt(i);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void EditSkillButton_Click(object sender, RoutedEventArgs e)
        {
            int i = SkillList.SelectedIndex;
            if (i == -1)
            { return; }
            Skill edited = (Skill)updated_config.Skills[i].Clone();
            SkillEditWindow charEdit = new SkillEditWindow(Height, Width, edited);
            var dialogResult = charEdit.ShowDialog();

            if (dialogResult == true)
            {
                updated_config.Skills[i] = edited;
                ConfigManager.SaveConfig(updated_config);
            }
        }

        public void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            int i = SkillList.SelectedIndex;
            int j = CharacterComboBox.SelectedIndex;
            int k = memberComboBox.SelectedIndex;
            if (i != -1)
            {
                if (j != -1)
                {
                    updated_config.Characters[j].Skills.Add(updated_config.Skills[i]);
                    ConfigManager.SaveConfig(updated_config);
                }
                if (k != -1)
                {
                    updated_config.Parties[partyComboBox.SelectedIndex].Members[k].Skills.Add(updated_config.Skills[i]);
                    ConfigManager.SaveConfig(updated_config);
                }
                
            }
        }
        public void CreateEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            Enemy newEnemy = new Enemy();
            newEnemy.Name = "Unknown";
            EnemyEditWindow newCharEdit = new EnemyEditWindow(Height, Width, newEnemy);
            var dialogResult = newCharEdit.ShowDialog();
            if (dialogResult == true)
            {
                updated_config.Enemies.Add(newEnemy);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void DeleteEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            int i = EnemyList.SelectedIndex;
            if (i != -1)
            {
                updated_config.Enemies.RemoveAt(i);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        public void EditEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            int i = EnemyList.SelectedIndex;
            if (i == -1)
            { return; }
            Enemy edited = (Enemy)updated_config.Enemies[i].Clone();
            EnemyEditWindow charEdit = new EnemyEditWindow(Height, Width, edited);
            var dialogResult = charEdit.ShowDialog();

            if (dialogResult == true)
            {
                updated_config.Enemies[i] = edited;
                ConfigManager.SaveConfig(updated_config);
            }
        }
        private void EditCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == characterEditButton)
            {
                if (CharacterComboBox.SelectedIndex == -1) { return; }
                int i = CharacterComboBox.SelectedIndex;
                Character edited = (Character)updated_config.Characters[i].Clone();
                CharacterEditWindow charEdit = new CharacterEditWindow(Height, Width, edited);
                var dialogResult = charEdit.ShowDialog();
                
                if (dialogResult == true)
                {
                    updated_config.Characters[i] = edited;
                    ConfigManager.SaveConfig(updated_config);
                }
            }
            else if (sender == memberEditButton)
            {
                if (memberComboBox.SelectedIndex == -1) { return; }
                int i = partyComboBox.SelectedIndex;
                int j = memberComboBox.SelectedIndex;
                if (i != -1 && j != -1)
                {
                    Character edited = (Character)updated_config.Parties[i].Members[j].Clone();
                    CharacterEditWindow charEdit = new CharacterEditWindow(Height, Width, edited);
                    var dialogResult = charEdit.ShowDialog();
                    if (dialogResult == true)
                    {
                        updated_config.Parties[i].Members[j] = edited;
                        ConfigManager.SaveConfig(updated_config);
                    }
                }
            }
        }
    }
}
