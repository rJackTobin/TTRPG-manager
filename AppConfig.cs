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
        {"BackgroundPath", ""}
    };

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
    }
    
}
