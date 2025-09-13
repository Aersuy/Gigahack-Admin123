using System;
using System.Collections.Generic;

namespace Scans.Audit.DataClasses
{
    public class AuditResult
    {
        public string Target { get; set; } = string.Empty;
        public DateTime ScanTime { get; set; } = DateTime.Now;
        public int OverallScore { get; set; }
        public ComplianceLevel OverallCompliance { get; set; } = ComplianceLevel.Red;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();

        // Security Categories
        public SecurityCategory NetworkSecurity { get; set; } = new SecurityCategory("Network Security");
        public SecurityCategory PasswordSecurity { get; set; } = new SecurityCategory("Password Security");
        public SecurityCategory SystemSecurity { get; set; } = new SecurityCategory("System Security");
        public SecurityCategory VulnerabilityManagement { get; set; } = new SecurityCategory("Vulnerability Management");
        public SecurityCategory WebSecurity { get; set; } = new SecurityCategory("Web Security");
        public SecurityCategory Compliance { get; set; } = new SecurityCategory("Compliance");
    }

    public class SecurityCategory
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public ComplianceLevel Level { get; set; } = ComplianceLevel.Red;
        public List<AuditItem> Items { get; set; } = new List<AuditItem>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();

        public SecurityCategory(string name)
        {
            Name = name;
        }
    }

    public class AuditItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplianceLevel Level { get; set; } = ComplianceLevel.Red;
        public bool Passed { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int Weight { get; set; } = 1; // Weight for scoring calculation
    }

    public enum ComplianceLevel
    {
        Red = 0,    // 0-40% - Critical issues
        Yellow = 1, // 41-70% - Needs improvement
        Green = 2   // 71-100% - Good compliance
    }

    public static class ComplianceLevelExtensions
    {
        public static string GetColor(this ComplianceLevel level)
        {
            return level switch
            {
                ComplianceLevel.Green => "#28a745", // Green
                ComplianceLevel.Yellow => "#ffc107", // Yellow
                ComplianceLevel.Red => "#dc3545", // Red
                _ => "#6c757d" // Gray
            };
        }

        public static string GetEmoji(this ComplianceLevel level)
        {
            return level switch
            {
                ComplianceLevel.Green => "🟢",
                ComplianceLevel.Yellow => "🟡",
                ComplianceLevel.Red => "🔴",
                _ => "⚪"
            };
        }

        public static string GetText(this ComplianceLevel level)
        {
            return level switch
            {
                ComplianceLevel.Green => "Good",
                ComplianceLevel.Yellow => "Needs Improvement",
                ComplianceLevel.Red => "Critical",
                _ => "Unknown"
            };
        }
    }
}
