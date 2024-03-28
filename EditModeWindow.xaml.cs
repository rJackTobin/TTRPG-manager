using System;
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

        public EditModeWindow(AppConfig config, double height, double width)
        {
            InitializeComponent();

            updated_config = config;
            // Or however you instantiate it

            // Copy values from old_config to new_config if needed

            this.DataContext = updated_config; // Set DataContext for data binding

            this.Height = height * 0.8;
            this.Width = width * 0.8;
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

        private void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            int i = partyComboBox.SelectedIndex;
            int j = memberComboBox.SelectedIndex;
            if (i != -1 && j != 1)
            {
                updated_config.Characters.Add(updated_config.Parties[i].Members[j]);
                updated_config.Parties[i].Members.RemoveAt(j);
                ConfigManager.SaveConfig(updated_config);
            }
        }
        private void EditCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == characterEditButton)
            {
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
                int i = partyComboBox.SelectedIndex;
                int j = memberComboBox.SelectedIndex;
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
