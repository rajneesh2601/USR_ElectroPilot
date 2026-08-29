using System;

namespace USR_ElectroPilot.Models
{
    public class IpDataMessageModel
    {
        public DateTime Timestamp { get; set; }
        public string Direction { get; set; }
        public string Text { get; set; }
        public string Hex { get; set; }
        public int ByteCount { get; set; }
    }
}
