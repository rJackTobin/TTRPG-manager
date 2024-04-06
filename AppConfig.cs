using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace TTRPG_manager
{
    public class AppConfig
    {


        // Removed the standalone Resolution and BackgroundPath properties.

        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>()
    {
        // Prepopulate the dictionary with default values for Resolution and BackgroundPath.
        {"Resolution", "1280x720"},
        {"BackgroundPath", ""},
        {"ScreenType", "Windowed" }

    };

        public int selectedPartyIndex { get; set; } = 0;
        public bool addedFirewallRule { get; set; } = false;

        public ObservableCollection<Party> Parties { get; set; } = new ObservableCollection<Party>();

        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();

        public ObservableCollection<Skill> Skills { get; set; } = new ObservableCollection<Skill>();

        public ObservableCollection<Character> Characters { get; set; } = new ObservableCollection<Character>();

        public ObservableCollection<StatusEffect> StatusEffects { get; set; } = new ObservableCollection<StatusEffect>();

        public ObservableCollection<Enemy> Enemies { get; set; } = new ObservableCollection<Enemy>();

        public ObservableCollection<string> LibraryPaths { get; set; } = new ObservableCollection<string>();
        // Convenience methods to access specific settings easily
        [JsonIgnore]
        public string Resolution
        {
            get => Settings.TryGetValue("Resolution", out var resolution) ? resolution.ToString() : "1280x720";
            set => Settings["Resolution"] = value;
        }
        [JsonIgnore]
        public string BackgroundPath
        {
            get => Settings.TryGetValue("BackgroundPath", out var backgroundPath) ? backgroundPath.ToString() : "";
            set => Settings["BackgroundPath"] = value;
        }
        [JsonIgnore]
        public string ScreenType
        {
            get => Settings.TryGetValue("ScreenType", out var screentype) ? screentype.ToString() : "";
            set => Settings["ScreenType"] = value;
        }
    }
}
