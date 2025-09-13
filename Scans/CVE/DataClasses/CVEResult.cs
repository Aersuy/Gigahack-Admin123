using System;
using System.Collections.Generic;

namespace Scans.CVE.DataClasses
{
    public class CVEResult
    {
        public string CVEId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public double? CVSSScore { get; set; }
        public string CVSSVector { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public List<string> References { get; set; } = new List<string>();
        public List<string> CWE { get; set; } = new List<string>();
        public List<string> AffectedProducts { get; set; } = new List<string>();
        public List<string> AffectedVersions { get; set; } = new List<string>();
        public string Source { get; set; } = string.Empty;
        public DateTime ScanTime { get; set; } = DateTime.Now;
    }

    public class CVESearchResult
    {
        public List<CVEResult> CVEs { get; set; } = new List<CVEResult>();
        public int TotalResults { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public DateTime SearchTime { get; set; } = DateTime.Now;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class CVEDatabaseResponse
    {
        public int resultsPerPage { get; set; }
        public int startIndex { get; set; }
        public int totalResults { get; set; }
        public string format { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public string timestamp { get; set; } = string.Empty;
        public List<CVEDatabaseVulnerability> vulnerabilities { get; set; } = new List<CVEDatabaseVulnerability>();
    }

    public class CVEDatabaseVulnerability
    {
        public CVEDatabaseItem cve { get; set; } = new CVEDatabaseItem();
    }

    public class CVEDatabaseItem
    {
        public string id { get; set; } = string.Empty;
        public string sourceIdentifier { get; set; } = string.Empty;
        public DateTime published { get; set; }
        public DateTime lastModified { get; set; }
        public string vulnStatus { get; set; } = string.Empty;
        public CVEDatabaseDescription descriptions { get; set; } = new CVEDatabaseDescription();
        public CVEDatabaseMetrics metrics { get; set; } = new CVEDatabaseMetrics();
        public CVEDatabaseWeaknesses weaknesses { get; set; } = new CVEDatabaseWeaknesses();
        public CVEDatabaseReferences references { get; set; } = new CVEDatabaseReferences();
    }

    public class CVEDatabaseDescription
    {
        public List<CVEDatabaseDescriptionItem> Lang { get; set; } = new List<CVEDatabaseDescriptionItem>();
    }

    public class CVEDatabaseDescriptionItem
    {
        public string Lang { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class CVEDatabaseMetrics
    {
        public List<CVEDatabaseCvssMetric> CvssMetricV31 { get; set; } = new List<CVEDatabaseCvssMetric>();
        public List<CVEDatabaseCvssMetric> CvssMetricV30 { get; set; } = new List<CVEDatabaseCvssMetric>();
        public List<CVEDatabaseCvssMetric> CvssMetricV2 { get; set; } = new List<CVEDatabaseCvssMetric>();
    }

    public class CVEDatabaseCvssMetric
    {
        public string Source { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public CVEDatabaseCvssData CvssData { get; set; } = new CVEDatabaseCvssData();
        public string ExploitabilityScore { get; set; } = string.Empty;
        public string ImpactScore { get; set; } = string.Empty;
    }

    public class CVEDatabaseCvssData
    {
        public string Version { get; set; } = string.Empty;
        public string VectorString { get; set; } = string.Empty;
        public string AttackVector { get; set; } = string.Empty;
        public string AttackComplexity { get; set; } = string.Empty;
        public string PrivilegesRequired { get; set; } = string.Empty;
        public string UserInteraction { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string ConfidentialityImpact { get; set; } = string.Empty;
        public string IntegrityImpact { get; set; } = string.Empty;
        public string AvailabilityImpact { get; set; } = string.Empty;
        public double BaseScore { get; set; }
        public string BaseSeverity { get; set; } = string.Empty;
    }

    public class CVEDatabaseWeaknesses
    {
        public List<CVEDatabaseWeaknessItem> Description { get; set; } = new List<CVEDatabaseWeaknessItem>();
    }

    public class CVEDatabaseWeaknessItem
    {
        public string Lang { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CVEDatabaseReferences
    {
        public List<CVEDatabaseReferenceItem> ReferenceData { get; set; } = new List<CVEDatabaseReferenceItem>();
    }

    public class CVEDatabaseReferenceItem
    {
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Refsource { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }
}
