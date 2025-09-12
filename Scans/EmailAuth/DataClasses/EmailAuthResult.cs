using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scans.EmailAuth.DataClasses
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
   
}
