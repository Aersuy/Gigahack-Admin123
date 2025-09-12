using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.EmailAuth.DataClasses
{
    public class SPFResult
    {
        public bool RecordExists { get; set; }
        public string Record { get; set; }
        public bool IsValid { get; set; }
        public List<string> Mechanisms { get; set; } = new List<string>();
        public List<string> Includes { get; set; } = new List<string>();
        public string Qualifier { get; set; } // +, -, ~, ?
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
