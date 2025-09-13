using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.DataClasses
{
    public class OverallReportData
    {
        public string Target { get; set; } = string.Empty;
        public int OverallScore { get; set; }
        public string ComplianceLevel { get; set; } = string.Empty;
        public DateTime ScanTime { get; set; }
        
        // Individual category scores
        public int NetworkSecurityScore { get; set; }
        public int SystemSecurityScore { get; set; }
        public int VulnerabilityManagementScore { get; set; }
        public int PasswordSecurityScore { get; set; }
        public int WebSecurityScore { get; set; }
        
        // Key findings and recommendations
        public List<string> KeyFindings { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        
        // Additional details
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
