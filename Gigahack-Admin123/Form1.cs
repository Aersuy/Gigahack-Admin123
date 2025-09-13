using Scans.InternetExposure.ScanLogic;
using Scans.InternetExposure.DataClases;
using Scans.CVE.ScanLogic;
using Scans.CVE.DataClasses;
using Scans.Password.PasswordLogic;
using Scans.Password.DataClasses;
using Scans.Audit.AuditLogic;
using Scans.Audit.DataClasses;
using LLM.Logic;
using LLM.DataClasses;
using System.Net;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Gigahack_Admin123
{
    public partial class Form1 : Form
    {
        private PortScanner portScanner;
        private EmailAuthScanner emailAuthScanner;
        private CVEScanner cveScanner;
        private PasswordChangeScan passwordPolicyScanner;
        private AuditScanner auditScanner;
        private Communicate llmCommunicate;
        private CancellationTokenSource? cancellationTokenSource;
        private int openPortsCount = 0;
        private int closedPortsCount = 0;
        private AuditResult? currentAuditResult;

        public Form1()
        {
            InitializeComponent();
            portScanner = new PortScanner();
            emailAuthScanner = new EmailAuthScanner();
            cveScanner = new CVEScanner();
            passwordPolicyScanner = new PasswordChangeScan();
            auditScanner = new AuditScanner();
            llmCommunicate = new Communicate();
            
            // Initialize score display
            UpdateOverallScoreDisplay(0, Scans.Audit.DataClasses.ComplianceLevel.Red);
            
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
            // Store the current audit result for report generation
            currentAuditResult = result;
            
            lstResults.Items.Clear();
            
            // Update the prominent score display
            UpdateOverallScoreDisplay(result.OverallScore, result.OverallCompliance);
            
            // Header with overall compliance
            lstResults.Items.Add($"=== 🔍 MINI-AUDIT DASHBOARD ===");
            lstResults.Items.Add($"Target: {result.Target}");
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
            DisplaySecurityCategory(result.WebSecurity);

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

        private void UpdateOverallScoreDisplay(int score, Scans.Audit.DataClasses.ComplianceLevel level)
        {
            // Update the large score value
            lblScoreValue.Text = score.ToString();
            
            // Set color based on compliance level
            switch (level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lblScoreValue.ForeColor = System.Drawing.Color.Green;
                    lblScoreStatus.Text = "🟢 Good Compliance";
                    lblScoreStatus.ForeColor = System.Drawing.Color.Green;
                    break;
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lblScoreValue.ForeColor = System.Drawing.Color.Orange;
                    lblScoreStatus.Text = "🟡 Needs Improvement";
                    lblScoreStatus.ForeColor = System.Drawing.Color.Orange;
                    break;
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lblScoreValue.ForeColor = System.Drawing.Color.Red;
                    lblScoreStatus.Text = "🔴 Critical Issues";
                    lblScoreStatus.ForeColor = System.Drawing.Color.Red;
                    break;
                default:
                    lblScoreValue.ForeColor = System.Drawing.Color.Gray;
                    lblScoreStatus.Text = "Not Scanned";
                    lblScoreStatus.ForeColor = System.Drawing.Color.Gray;
                    break;
            }
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (currentAuditResult == null)
            {
                MessageBox.Show("Please run a security audit first before generating a report.", "No Audit Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnGenerateReport.Enabled = false;
            lblStatus.Text = "Generating Word report...";

            try
            {
                Task.Run(async () => {
                    try
                    {
                        // Convert audit result to report data
                        var reportData = ConvertAuditResultToReportData(currentAuditResult);
                        
                        // Generate report using LLM
                        var report = await llmCommunicate.GenerateReport(reportData);

                        this.Invoke(new Action(() => {
                            DisplayGeneratedReport(report);
                            lblStatus.Text = "Word report generated successfully";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => {
                            MessageBox.Show($"Report generation error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "Word report generation failed";
                        }));
                    }
                    finally
                    {
                        this.Invoke(new Action(() => {
                            btnGenerateReport.Enabled = true;
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Report generation error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Word report generation failed";
                btnGenerateReport.Enabled = true;
            }
        }

        private OverallReportData ConvertAuditResultToReportData(AuditResult auditResult)
        {
            var reportData = new OverallReportData
            {
                Target = auditResult.Target,
                OverallScore = auditResult.OverallScore,
                ComplianceLevel = auditResult.OverallCompliance.GetText(),
                ScanTime = auditResult.ScanTime,
                Success = auditResult.Success,
                ErrorMessage = auditResult.ErrorMessage,
                
                // Category scores
                NetworkSecurityScore = auditResult.NetworkSecurity.Score,
                SystemSecurityScore = auditResult.SystemSecurity.Score,
                VulnerabilityManagementScore = auditResult.VulnerabilityManagement.Score,
                PasswordSecurityScore = auditResult.PasswordSecurity.Score,
                WebSecurityScore = auditResult.WebSecurity.Score,
                
                // Key findings from all categories
                KeyFindings = new List<string>(),
                Recommendations = new List<string>(),
                Warnings = new List<string>(),
                Errors = new List<string>()
            };

            // Collect key findings from all categories
            foreach (var category in new[] { auditResult.NetworkSecurity, auditResult.SystemSecurity, 
                                          auditResult.VulnerabilityManagement, auditResult.PasswordSecurity, 
                                          auditResult.WebSecurity })
            {
                foreach (var item in category.Items)
                {
                    if (!item.Passed)
                    {
                        reportData.KeyFindings.Add($"{category.Name}: {item.Name} - {item.Status}");
                    }
                }
                
                reportData.Recommendations.AddRange(category.Recommendations);
                reportData.Warnings.AddRange(category.Errors);
            }

            // Add overall recommendations
            reportData.Recommendations.AddRange(auditResult.Recommendations);
            reportData.Warnings.AddRange(auditResult.Warnings);
            reportData.Errors.AddRange(auditResult.Errors);

            return reportData;
        }

        private void DisplayGeneratedReport(string report)
        {
            try
            {
                // Create a save file dialog for Word document
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Word Documents (*.docx)|*.docx|All Files (*.*)|*.*";
                    saveFileDialog.FileName = $"Security_Report_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                    saveFileDialog.Title = "Save Security Report";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Generate Word document
                        GenerateWordDocument(report, saveFileDialog.FileName);
                        
                        // Show success message
                        MessageBox.Show($"Security report saved successfully!\n\nLocation: {saveFileDialog.FileName}", 
                                      "Report Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Optionally open the document
                        var result = MessageBox.Show("Would you like to open the generated report?", 
                                                   "Open Report", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = saveFileDialog.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Word document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateWordDocument(string report, string filePath)
        {
            using (var wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add main document part
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Add title
                var titleParagraph = body.AppendChild(new Paragraph());
                var titleRun = titleParagraph.AppendChild(new Run());
                titleRun.AppendChild(new RunProperties(new Bold()));
                titleRun.AppendChild(new Text("Security Audit Report"));
                
                // Add title formatting
                titleParagraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { After = "200" }
                );

                // Add date
                var dateParagraph = body.AppendChild(new Paragraph());
                var dateRun = dateParagraph.AppendChild(new Run());
                dateRun.AppendChild(new Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"));
                dateParagraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { After = "400" }
                );

                // Add separator line
                body.AppendChild(new Paragraph(new Run(new Text(""))));
                body.AppendChild(new Paragraph(new Run(new Text("═══════════════════════════════════════════════════════════════"))));
                body.AppendChild(new Paragraph(new Run(new Text(""))));

                // Split report into sections and format
                var sections = report.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var section in sections)
                {
                    if (string.IsNullOrWhiteSpace(section)) continue;

                    var paragraph = body.AppendChild(new Paragraph());
                    var run = paragraph.AppendChild(new Run());

                    // Check if this is a header (starts with # or is all caps)
                    if (section.StartsWith("#") || (section.Length < 50 && section.All(c => char.IsUpper(c) || char.IsWhiteSpace(c))))
                    {
                        // Format as header
                        run.AppendChild(new RunProperties(new Bold()));
                        run.AppendChild(new Text(section.TrimStart('#', ' ')));
                        paragraph.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "200", After = "100" }
                        );
                    }
                    else
                    {
                        // Format as regular text
                        run.AppendChild(new Text(section));
                        paragraph.ParagraphProperties = new ParagraphProperties(
                            new SpacingBetweenLines() { After = "100" }
                        );
                    }
                }

                // Add footer
                body.AppendChild(new Paragraph(new Run(new Text(""))));
                body.AppendChild(new Paragraph(new Run(new Text("═══════════════════════════════════════════════════════════════"))));
                body.AppendChild(new Paragraph(new Run(new Text(""))));
                
                var footerParagraph = body.AppendChild(new Paragraph());
                var footerRun = footerParagraph.AppendChild(new Run());
                footerRun.AppendChild(new Text("This report was generated by the Security Audit Dashboard"));
                footerParagraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { Before = "200" }
                );
            }
        }

        private void btnQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                var quizForm = new QuizForm();
                quizForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening quiz: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
