using System;

namespace USR_ElectroPilot.Models
{
    public class SystemSettingModel
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
