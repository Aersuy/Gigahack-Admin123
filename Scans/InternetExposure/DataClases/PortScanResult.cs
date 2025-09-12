using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.InternetExposure.DataClases
{
    public class PortScanResult
    {
        public required List<PortScanItem> Ports;

    }
    public class PortScanItem
    {
        public int portNumber { get; set; }
        public string service { get; set; } = "Unknown";
        public bool open { get; set; }
        public string banner { get; set; } = "";
        public string protocol { get; set; } = "TCP";
        public DateTime scanTime { get; set; } = DateTime.Now;
    }
}
