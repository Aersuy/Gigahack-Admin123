using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.EmailAuth.DataClasses
{
    public class DKIMResult
    {
        public string Selector { get; set; }
        public string Domain { get; set; }
        public bool RecordExists { get; set; }
        public string PublicKey { get; set; }
        public int KeyLength { get; set; }
        public string KeyType { get; set; } // RSA, ED25519
        public bool IsValid { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
