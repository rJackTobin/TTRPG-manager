using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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
        {"LibraryPaths", "" }
    };

    public List<Party> Parties { get; set; } = new List<Party>();

    public List<Item> Items { get; set; } = new List<Item> ();

    public List<Skill> Skills { get; set; } = new List<Skill>();

    public List<Character> Characters { get; set; } = new List<Character> ();

    public List<StatusEffect> StatusEffects { get; set; } = new List<StatusEffect>();

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
    public string LibraryPaths
    { 
        get => Settings.TryGetValue("LibraryPaths", out var librarypaths) ? librarypaths.ToString() : "";
        set => Settings["LibraryPaths"] = value;
    }
    }
    
}
