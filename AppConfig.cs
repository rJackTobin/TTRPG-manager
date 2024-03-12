using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    public class AppConfig
    {
        public string SelectedResolution { get; set; } = "1280x720";

        public string BackgroundPath { get; set; } = null;

        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    }
}
