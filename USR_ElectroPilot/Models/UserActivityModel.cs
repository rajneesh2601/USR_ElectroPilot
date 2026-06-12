using System;

namespace USR_ElectroPilot.Models
{
    public class UserActivityModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string ActivityType { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
