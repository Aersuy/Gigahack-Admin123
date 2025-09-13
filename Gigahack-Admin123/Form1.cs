using Scans.InternetExposure.ScanLogic;
using Scans.InternetExposure.DataClases;
using Scans.CVE.ScanLogic;
using Scans.CVE.DataClasses;
using Scans.Password.PasswordLogic;
using Scans.Password.DataClasses;
using Scans.Audit.AuditLogic;
using Scans.Audit.DataClasses;
using System.Net;

namespace Gigahack_Admin123
{
    public partial class Form1 : Form
    {
        private PortScanner portScanner;
        private EmailAuthScanner emailAuthScanner;
        private CVEScanner cveScanner;
        private PasswordChangeScan passwordPolicyScanner;
        private AuditScanner auditScanner;
        private CancellationTokenSource? cancellationTokenSource;
        private int openPortsCount = 0;
        private int closedPortsCount = 0;

        public Form1()
        {
            InitializeComponent();
            portScanner = new PortScanner();
            emailAuthScanner = new EmailAuthScanner();
            cveScanner = new CVEScanner();
            passwordPolicyScanner = new PasswordChangeScan();
            auditScanner = new AuditScanner();
            
            // Add cleanup on form close
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cveScanner?.Dispose();
            auditScanner?.Dispose();
        }





        private void btnStop_Click(object sender, EventArgs e)
        {
            // For synchronous scanning, you might want to add a cancellation flag
            lblStatus.Text = "Stopping scan...";
            // Note: You'll need to add cancellation logic to your AllPortScan method for this to work properly
        }

        private void UpdatePortCounts()
        {
            lblOpenPorts.Text = $"Open: {openPortsCount}";
            lblClosedPorts.Text = $"Closed: {closedPortsCount}";
        }



        // Audit Dashboard Methods

        private void DisplayCVEResults(CVESearchResult result)
        {
            lstResults.Items.Clear();
            
            lstResults.Items.Add($"=== CVE Search Results ===");
            lstResults.Items.Add($"Search Query: {result.SearchQuery}");
            lstResults.Items.Add($"Total CVEs Found: {result.TotalResults}");
            lstResults.Items.Add($"Search Time: {result.SearchTime:yyyy-MM-dd HH:mm:ss}");
            lstResults.Items.Add("");

            if (!result.Success)
            {
                lstResults.Items.Add($"❌ Error: {result.ErrorMessage}");
                return;
            }

            if (result.CVEs.Count == 0)
            {
                lstResults.Items.Add("✅ No CVEs found for the discovered services.");
                return;
            }

            // Group CVEs by severity
            var criticalCVEs = result.CVEs.Where(c => c.Severity?.ToLower() == "critical").ToList();
            var highCVEs = result.CVEs.Where(c => c.Severity?.ToLower() == "high").ToList();
            var mediumCVEs = result.CVEs.Where(c => c.Severity?.ToLower() == "medium").ToList();
            var lowCVEs = result.CVEs.Where(c => c.Severity?.ToLower() == "low").ToList();
            var otherCVEs = result.CVEs.Where(c => !new[] { "critical", "high", "medium", "low" }.Contains(c.Severity?.ToLower())).ToList();

            // Display summary
            lstResults.Items.Add("=== Summary ===");
            if (criticalCVEs.Any()) lstResults.Items.Add($"🔴 Critical: {criticalCVEs.Count}");
            if (highCVEs.Any()) lstResults.Items.Add($"🟠 High: {highCVEs.Count}");
            if (mediumCVEs.Any()) lstResults.Items.Add($"🟡 Medium: {mediumCVEs.Count}");
            if (lowCVEs.Any()) lstResults.Items.Add($"🟢 Low: {lowCVEs.Count}");
            if (otherCVEs.Any()) lstResults.Items.Add($"⚪ Other: {otherCVEs.Count}");
            lstResults.Items.Add("");

            // Display CVEs by severity
            DisplayCVESeverityGroup("Critical", criticalCVEs, "🔴");
            DisplayCVESeverityGroup("High", highCVEs, "🟠");
            DisplayCVESeverityGroup("Medium", mediumCVEs, "🟡");
            DisplayCVESeverityGroup("Low", lowCVEs, "🟢");
            DisplayCVESeverityGroup("Other", otherCVEs, "⚪");
        }

        private void DisplayCVESeverityGroup(string severity, List<CVEResult> cves, string emoji)
        {
            if (!cves.Any()) return;

            lstResults.Items.Add($"=== {severity} Severity CVEs ===");
            
            foreach (var cve in cves.Take(10)) // Limit to top 10 per severity
            {
                lstResults.Items.Add($"{emoji} {cve.CVEId}");
                lstResults.Items.Add($"   Score: {cve.CVSSScore:F1} | Severity: {cve.Severity}");
                lstResults.Items.Add($"   Published: {cve.PublishedDate:yyyy-MM-dd}");
                
                // Truncate description for display
                var description = cve.Description.Length > 100 ? 
                    cve.Description.Substring(0, 100) + "..." : 
                    cve.Description;
                lstResults.Items.Add($"   Description: {description}");
                
                if (cve.AffectedProducts.Any())
                {
                    lstResults.Items.Add($"   Affected: {string.Join(", ", cve.AffectedProducts.Take(3))}");
                }
                
                if (cve.References.Any())
                {
                    lstResults.Items.Add($"   References: {cve.References.First()}");
                }
                
                lstResults.Items.Add("");
            }

            if (cves.Count > 10)
            {
                lstResults.Items.Add($"   ... and {cves.Count - 10} more {severity.ToLower()} severity CVEs");
                lstResults.Items.Add("");
            }
        }



        // Audit Dashboard Methods
        private void btnAuditDashboard_Click(object sender, EventArgs e)
        {
            btnAuditDashboard.Enabled = false;
            lblStatus.Text = "Running comprehensive security audit...";

            try
            {
                Task.Run(async () => {
                    try
                    {
                        var targetIP = !string.IsNullOrEmpty(txtTargetIP.Text) ? txtTargetIP.Text : "127.0.0.1";
                        var result = await auditScanner.PerformMiniAudit(targetIP);

                        this.Invoke(new Action(() => {
                            DisplayAuditDashboardResults(result);
                            lblStatus.Text = "Security audit completed";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => {
                            MessageBox.Show($"Audit error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "Audit failed";
                        }));
                    }
                    finally
                    {
                    this.Invoke(new Action(() => {
                            btnAuditDashboard.Enabled = true;
                    }));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Audit error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Audit failed";
                btnAuditDashboard.Enabled = true;
            }
        }

        private void DisplayAuditDashboardResults(AuditResult result)
        {
            lstResults.Items.Clear();
            
            // Header with overall compliance
            lstResults.Items.Add($"=== 🔍 MINI-AUDIT DASHBOARD ===");
            lstResults.Items.Add($"Target: {result.Target}");
            lstResults.Items.Add($"Overall Score: {result.OverallScore}/100 {result.OverallCompliance.GetEmoji()} {result.OverallCompliance.GetText()}");
            lstResults.Items.Add($"Scan Time: {result.ScanTime:yyyy-MM-dd HH:mm:ss}");
            lstResults.Items.Add("");
            
            if (!result.Success)
            {
                lstResults.Items.Add($"❌ Error: {result.ErrorMessage}");
                return;
            }

            // Display each security category
            DisplaySecurityCategory(result.NetworkSecurity);
            DisplaySecurityCategory(result.SystemSecurity);
            DisplaySecurityCategory(result.VulnerabilityManagement);
            DisplaySecurityCategory(result.PasswordSecurity);

            // Overall Recommendations
            if (result.Recommendations.Any())
            {
                lstResults.Items.Add("=== 📋 OVERALL RECOMMENDATIONS ===");
                foreach (var recommendation in result.Recommendations)
                {
                    lstResults.Items.Add(recommendation);
            }
            lstResults.Items.Add("");
            }

            // Warnings
            if (result.Warnings.Any())
            {
                lstResults.Items.Add("=== ⚠️ WARNINGS ===");
                foreach (var warning in result.Warnings)
                {
                    lstResults.Items.Add($"⚠️ {warning}");
                }
                lstResults.Items.Add("");
            }
            
            // Errors
            if (result.Errors.Any())
            {
                lstResults.Items.Add("=== ❌ ERRORS ===");
                foreach (var error in result.Errors)
                {
                    lstResults.Items.Add($"❌ {error}");
                }
            }
        }

        private void DisplaySecurityCategory(SecurityCategory category)
        {
            lstResults.Items.Add($"=== {category.Level.GetEmoji()} {category.Name.ToUpper()} ===");
            lstResults.Items.Add($"Score: {category.Score}/100 {category.Level.GetEmoji()} {category.Level.GetText()}");
            lstResults.Items.Add("");

            foreach (var item in category.Items)
            {
                var statusIcon = item.Passed ? "✅" : "❌";
                lstResults.Items.Add($"{statusIcon} {item.Name}");
                lstResults.Items.Add($"   {item.Description}");
                lstResults.Items.Add($"   Status: {item.Status}");
                if (!string.IsNullOrEmpty(item.Details))
                {
                    lstResults.Items.Add($"   Details: {item.Details}");
                }
                lstResults.Items.Add("");
            }

            if (category.Recommendations.Any())
            {
                lstResults.Items.Add("   Recommendations:");
                foreach (var rec in category.Recommendations)
                {
                    lstResults.Items.Add($"   💡 {rec}");
                }
                lstResults.Items.Add("");
            }
        }

    }
}
