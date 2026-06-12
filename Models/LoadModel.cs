using System;

namespace USR_ElectroPilot.Models
{
    public class LoadModel
    {
        public int Id { get; set; }
        public string LoadNumber { get; set; }
        public int? RecipeId { get; set; }
        public int? WagonId { get; set; }
        public string Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
