using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.InternetExposure.DataClases
{
    public class EmailAuthResult
    {
        public string Domain { get; set; }
        public DateTime ScanTime { get; set; } = DateTime.Now;
        
        // SPF Results
        public SPFResult SPF { get; set; } = new SPFResult();
        
        // DKIM Results
        public List<DKIMResult> DKIM { get; set; } = new List<DKIMResult>();
        
        // DMARC Results
        public DMARCResult DMARC { get; set; } = new DMARCResult();
        
        // Overall Security Score
        public int SecurityScore { get; set; }
        public string SecurityGrade { get; set; } // A, B, C, D, F
    }

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

    public class DKIMResult
    {
        public string Selector { get; set; }
        public string Domain { get; set; }
        public bool RecordExists { get; set; }
        public string PublicKey { get; set; }
        public int KeyLength { get; set; }
        public string KeyType { get; set; } // RSA, ED25519
        public bool IsValid { get; set; }
        public string Algorithm { get; set; }
        public string HashAlgorithm { get; set; }
        public string ServiceType { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

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
