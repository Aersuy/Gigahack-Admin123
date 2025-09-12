using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.EmailAuth.DataClasses
{
    public class DMARCResult
    {
        public bool RecordExists { get; set; }
        public string Record { get; set; }
        public bool IsValid { get; set; }
        public string Policy { get; set; } // none, quarantine, reject
        public string SubdomainPolicy { get; set; }
        public int Percentage { get; set; } = 100;
        public string RUA { get; set; } // Aggregate reports
        public string RUF { get; set; } // Forensic reports
        public string Alignment { get; set; } // strict, relaxed
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
