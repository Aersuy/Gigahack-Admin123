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
        
        // Category-specific detailed suggestions
        public CategorySuggestions NetworkSecuritySuggestions { get; set; } = new CategorySuggestions();
        public CategorySuggestions SystemSecuritySuggestions { get; set; } = new CategorySuggestions();
        public CategorySuggestions VulnerabilityManagementSuggestions { get; set; } = new CategorySuggestions();
        public CategorySuggestions PasswordSecuritySuggestions { get; set; } = new CategorySuggestions();
        public CategorySuggestions WebSecuritySuggestions { get; set; } = new CategorySuggestions();
        
        // Assessment data for LLM integration
        public AssessmentResult? Assessment { get; set; }
        public bool HasAssessmentData => Assessment != null;
    }

    public class CategorySuggestions
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string ComplianceLevel { get; set; } = string.Empty;
        public List<string> ImmediateSuggestions { get; set; } = new List<string>();
        public List<string> BestPractices { get; set; } = new List<string>();
        public List<string> CriticalActions { get; set; } = new List<string>();
        public string SummaryMessage { get; set; } = string.Empty;
    }
}
