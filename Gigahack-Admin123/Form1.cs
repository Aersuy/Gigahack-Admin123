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
        private AuditResult? currentAuditResult;
        private LLM.DataClasses.AssessmentResult? currentAssessmentResult;
        
        // Workflow state tracking
        private bool isAssessmentCompleted = false;
        private bool isScanCompleted = false;

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
            
            // Initialize workflow state
            UpdateWorkflowState();
            
            // Make buttons rounded with proper spacing
            MakeButtonsRounded();
            
            // Ensure all button text is white
            EnsureButtonTextIsWhite();
            
            // Add cleanup on form close
            this.FormClosed += Form1_FormClosed;
            
            // Apply Windows 11 Dark Mode Theme AFTER everything is initialized
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Apply dark mode theme after the form is fully loaded
            ApplyDarkModeTheme();
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cveScanner?.Dispose();
            auditScanner?.Dispose();
        }

        /// <summary>
        /// Apply Windows 11 Dark Mode theme to the main form
        /// </summary>
        private void ApplyDarkModeTheme()
        {
            // Forcefully apply the dark mode theme to override Designer colors
            DarkModeTheme.ForceApplyColors(this);
            
            // Apply rounded corners to buttons with Windows 11 styling
            DarkModeTheme.ApplyRoundedCorners(btnAuditDashboard, 8);
            DarkModeTheme.ApplyRoundedCorners(btnGenerateReport, 8);
            DarkModeTheme.ApplyRoundedCorners(btnQuiz, 8);
            DarkModeTheme.ApplyRoundedCorners(btnClearResults, 8);
            
            // Apply rounded corners to input controls
            DarkModeTheme.ApplyRoundedCorners(txtTargetIP, 6);
            DarkModeTheme.ApplyRoundedCorners(lstResults, 8);
            
            // Apply refined progress bar styling
            DarkModeTheme.ApplyToProgressBar(progressBar);
        }







        private void SetProgressBarColor(System.Drawing.Color color)
        {
            // Use Windows API to set progress bar color to green
            try
            {
                // Send a message to set the progress bar color
                // PBM_SETBARCOLOR = 0x409, Green = 0x00FF00
                if (color == System.Drawing.Color.Green)
                {
                    SendMessage(progressBar.Handle, 0x409, 0, 0x00FF00);
                }
                else if (color == System.Drawing.Color.Red)
                {
                    SendMessage(progressBar.Handle, 0x409, 0, 0xFF0000); // Red in BGR format
                }
                else
                {
                    // Reset to default
                    SendMessage(progressBar.Handle, 0x409, 0, -1);
                }
            }
            catch
            {
                // Fallback: just continue without color change if API fails
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void MakeButtonRounded(Button button, int radius = 15)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            button.Region = new Region(path);
        }

        private void MakeButtonsRounded()
        {
            // Apply rounded corners to all buttons (keeping only rounded corners)
            MakeButtonRounded(btnAuditDashboard, 12);
            MakeButtonRounded(btnGenerateReport, 12);
            MakeButtonRounded(btnQuiz, 12);
            MakeButtonRounded(btnClearResults, 12);
            
            // Also round the text input and results list
            MakeControlRounded(txtTargetIP, 8);
            MakeControlRounded(lstResults, 10);
        }

        private void MakeControlRounded(System.Windows.Forms.Control control, int radius = 15)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void EnsureButtonTextIsWhite()
        {
            // Force all button text to be white, regardless of enabled state
            btnAuditDashboard.ForeColor = System.Drawing.Color.White;
            btnGenerateReport.ForeColor = System.Drawing.Color.White;
            btnQuiz.ForeColor = System.Drawing.Color.White;
            btnClearResults.ForeColor = System.Drawing.Color.White;
        }


        private void UpdateWorkflowState()
        {
            // Update button states based on workflow progress
            btnAuditDashboard.Enabled = isAssessmentCompleted;
            btnGenerateReport.Enabled = isAssessmentCompleted && isScanCompleted;
            
            // Force white text color for all buttons regardless of enabled state
            btnAuditDashboard.ForeColor = System.Drawing.Color.White;
            btnGenerateReport.ForeColor = System.Drawing.Color.White;
            btnQuiz.ForeColor = System.Drawing.Color.White;
            btnClearResults.ForeColor = System.Drawing.Color.White;
            
            // Update button text to show workflow requirements
            if (!isAssessmentCompleted)
            {
                btnAuditDashboard.Text = "🔒 Complete Assessment First";
                btnGenerateReport.Text = "🔒 Complete Assessment & Scan";
            }
            else if (!isScanCompleted)
            {
                btnAuditDashboard.Text = "🔍 Run Security Audit";
                btnGenerateReport.Text = "🔒 Complete Scan First";
            }
            else
            {
                btnAuditDashboard.Text = "🔍 Run Security Audit";
                btnGenerateReport.Text = "📄 Generate Word Report";
            }
            
            // Force white text again after text changes (in case system overrides it)
            EnsureButtonTextIsWhite();
            
            // Update status message to guide user
            if (!isAssessmentCompleted)
            {
                lblStatus.Text = "Step 1: Complete IT Infrastructure Assessment to begin";
            }
            else if (!isScanCompleted)
            {
                lblStatus.Text = "Step 2: Run security audit to analyze your infrastructure";
            }
            else
            {
                lblStatus.Text = "Step 3: Generate comprehensive Word report with findings";
            }
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
            // Check if assessment is completed first
            if (!isAssessmentCompleted)
            {
                MessageBox.Show("Please complete the IT Infrastructure Assessment first before running the security audit.", 
                              "Assessment Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            btnAuditDashboard.Enabled = false;
            lblStatus.Text = "Initializing security audit...";
            
            // Reset and start progress bar with green color
            progressBar.Value = 0;
            SetProgressBarColor(System.Drawing.Color.Green);

            try
            {
                Task.Run(async () => {
                    try
                    {
                        var targetIP = !string.IsNullOrEmpty(txtTargetIP.Text) ? txtTargetIP.Text : "127.0.0.1";
                        
                        // Update progress during different phases of the audit
                        this.Invoke(new Action(() => {
                            progressBar.Value = 10;
                            lblStatus.Text = "Starting network scan...";
                        }));
                        
                        await Task.Delay(500); // Small delay to show progress
                        
                        this.Invoke(new Action(() => {
                            progressBar.Value = 25;
                            lblStatus.Text = "Scanning network ports...";
                        }));
                        
                        // Start the actual audit
                        var result = await auditScanner.PerformMiniAudit(targetIP);
                        
                        // Simulate progress updates during scan phases
                        this.Invoke(new Action(() => {
                            progressBar.Value = 50;
                            lblStatus.Text = "Analyzing email security...";
                        }));
                        
                        await Task.Delay(300);
                        
                        this.Invoke(new Action(() => {
                            progressBar.Value = 70;
                            lblStatus.Text = "Checking password policies...";
                        }));
                        
                        await Task.Delay(300);
                        
                        this.Invoke(new Action(() => {
                            progressBar.Value = 85;
                            lblStatus.Text = "Scanning for vulnerabilities...";
                        }));
                        
                        await Task.Delay(300);
                        
                        this.Invoke(new Action(() => {
                            progressBar.Value = 95;
                            lblStatus.Text = "Generating security report...";
                        }));
                        
                        await Task.Delay(200);

                        this.Invoke(new Action(() => {
                            progressBar.Value = 100;
                            DisplayAuditDashboardResults(result);
                            lblStatus.Text = "Security audit completed successfully";
                            
                            // Mark scan as completed
                            isScanCompleted = true;
                            UpdateWorkflowState();
                        }));
                        
                        // Keep progress bar full for a moment, then reset
                        await Task.Delay(2000);
                        this.Invoke(new Action(() => {
                            progressBar.Value = 0;
                            SetProgressBarColor(System.Drawing.Color.Gray); // Reset to default color
                }));
            }
            catch (Exception ex)
            {
                        this.Invoke(new Action(() => {
                            progressBar.Value = 0;
                            SetProgressBarColor(System.Drawing.Color.Red); // Show red for error
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
                progressBar.Value = 0;
                SetProgressBarColor(System.Drawing.Color.Red);
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
            
            // Add category-specific suggestions below the score
            if (category.Name.Equals("Password Security", StringComparison.OrdinalIgnoreCase))
            {
                AddPasswordSecuritySuggestions(category);
            }
            else if (category.Name.Equals("Web Security", StringComparison.OrdinalIgnoreCase))
            {
                AddWebSecuritySuggestions(category);
            }
            else if (category.Name.Equals("Network Security", StringComparison.OrdinalIgnoreCase))
            {
                AddNetworkSecuritySuggestions(category);
            }
            else if (category.Name.Equals("System Security", StringComparison.OrdinalIgnoreCase))
            {
                AddSystemSecuritySuggestions(category);
            }
            else if (category.Name.Equals("Vulnerability Management", StringComparison.OrdinalIgnoreCase))
            {
                AddVulnerabilityManagementSuggestions(category);
            }
            
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
                lstResults.Items.Add("   📋 Recommendations:");
                foreach (var rec in category.Recommendations)
                {
                    lstResults.Items.Add($"   💡 {rec}");
                }
                lstResults.Items.Add("");
            }
        }

        private void AddPasswordSecuritySuggestions(SecurityCategory category)
        {
            lstResults.Items.Add("   🔐 Password Security Suggestions:");
            
            // Add debug information to see what's happening
            lstResults.Items.Add($"   Debug: Score={category.Score}, Level={category.Level}, Name='{category.Name}'");
            
            // Provide suggestions based on the score level
            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lstResults.Items.Add("   ✅ Excellent password security posture!");
                    lstResults.Items.Add("   • Consider implementing passwordless authentication where possible");
                    lstResults.Items.Add("   • Regular security awareness training for users");
                    lstResults.Items.Add("   • Monitor for compromised credentials in data breaches");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lstResults.Items.Add("   ⚠️ Good foundation, but improvements needed:");
                    lstResults.Items.Add("   • Enable Multi-Factor Authentication (MFA) for all accounts");
                    lstResults.Items.Add("   • Implement password complexity requirements");
                    lstResults.Items.Add("   • Set up account lockout policies");
                    lstResults.Items.Add("   • Regular password expiration (90-180 days)");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                default: // Handle any edge cases including score of 0
                    lstResults.Items.Add("   🚨 CRITICAL: Immediate password security improvements required:");
                    lstResults.Items.Add("   • URGENT: Enable Multi-Factor Authentication immediately");
                    lstResults.Items.Add("   • Enforce minimum 12-character passwords");
                    lstResults.Items.Add("   • Require uppercase, lowercase, numbers, and special characters");
                    lstResults.Items.Add("   • Implement account lockout after 5 failed attempts");
                    lstResults.Items.Add("   • Enable password history (remember last 12 passwords)");
                    lstResults.Items.Add("   • Force password changes for all users within 30 days");
                    break;
            }
            
            // Add general best practices
            lstResults.Items.Add("   📚 Best Practices:");
            lstResults.Items.Add("   • Use a reputable password manager");
            lstResults.Items.Add("   • Never reuse passwords across different systems");
            lstResults.Items.Add("   • Regular security awareness training");
            lstResults.Items.Add("   • Monitor for suspicious login activities");
        }

        private void AddWebSecuritySuggestions(SecurityCategory category)
        {
            lstResults.Items.Add("   🌐 Web Security Suggestions:");
            
            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lstResults.Items.Add("   ✅ Strong web security implementation!");
                    lstResults.Items.Add("   • Consider implementing Content Security Policy (CSP)");
                    lstResults.Items.Add("   • Regular security header audits");
                    lstResults.Items.Add("   • Monitor for new web vulnerabilities");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lstResults.Items.Add("   ⚠️ Web security needs attention:");
                    lstResults.Items.Add("   • Ensure all traffic uses HTTPS with valid certificates");
                    lstResults.Items.Add("   • Implement security headers (HSTS, X-Frame-Options)");
                    lstResults.Items.Add("   • Regular TLS configuration reviews");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lstResults.Items.Add("   🚨 CRITICAL: Web security vulnerabilities detected:");
                    lstResults.Items.Add("   • URGENT: Enable HTTPS for all web applications");
                    lstResults.Items.Add("   • Install valid SSL/TLS certificates");
                    lstResults.Items.Add("   • Configure security headers immediately");
                    lstResults.Items.Add("   • Disable weak TLS protocols (TLS 1.0, 1.1)");
                    break;
            }
        }

        private void AddNetworkSecuritySuggestions(SecurityCategory category)
        {
            lstResults.Items.Add("   🛡️ Network Security Suggestions:");
            
            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lstResults.Items.Add("   ✅ Strong network security posture!");
                    lstResults.Items.Add("   • Continue monitoring for new threats");
                    lstResults.Items.Add("   • Regular firewall rule reviews");
                    lstResults.Items.Add("   • Consider network segmentation improvements");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lstResults.Items.Add("   ⚠️ Network security improvements needed:");
                    lstResults.Items.Add("   • Close unnecessary open ports");
                    lstResults.Items.Add("   • Implement proper firewall rules");
                    lstResults.Items.Add("   • Regular network vulnerability scanning");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lstResults.Items.Add("   🚨 CRITICAL: Network security issues found:");
                    lstResults.Items.Add("   • URGENT: Close high-risk open ports");
                    lstResults.Items.Add("   • Implement network firewall immediately");
                    lstResults.Items.Add("   • Disable unnecessary network services");
                    lstResults.Items.Add("   • Enable network intrusion detection");
                    break;
            }
        }

        private void AddSystemSecuritySuggestions(SecurityCategory category)
        {
            lstResults.Items.Add("   💻 System Security Suggestions:");
            
            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lstResults.Items.Add("   ✅ Strong system security configuration!");
                    lstResults.Items.Add("   • Continue regular system monitoring");
                    lstResults.Items.Add("   • Implement advanced threat detection");
                    lstResults.Items.Add("   • Regular security baseline reviews");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lstResults.Items.Add("   ⚠️ System security needs improvements:");
                    lstResults.Items.Add("   • Enable automatic security updates");
                    lstResults.Items.Add("   • Implement endpoint detection and response (EDR)");
                    lstResults.Items.Add("   • Regular system hardening reviews");
                    lstResults.Items.Add("   • Configure centralized logging");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lstResults.Items.Add("   🚨 CRITICAL: System security vulnerabilities found:");
                    lstResults.Items.Add("   • URGENT: Install security patches immediately");
                    lstResults.Items.Add("   • Enable Windows Defender or equivalent antivirus");
                    lstResults.Items.Add("   • Disable unnecessary system services");
                    lstResults.Items.Add("   • Configure Windows Firewall");
                    lstResults.Items.Add("   • Enable User Account Control (UAC)");
                    break;
            }
            
            lstResults.Items.Add("   📚 Best Practices:");
            lstResults.Items.Add("   • Regular system updates and patches");
            lstResults.Items.Add("   • Principle of least privilege for user accounts");
            lstResults.Items.Add("   • Regular security scanning and monitoring");
            lstResults.Items.Add("   • Backup and disaster recovery planning");
        }

        private void AddVulnerabilityManagementSuggestions(SecurityCategory category)
        {
            lstResults.Items.Add("   🔍 Vulnerability Management Suggestions:");
            
            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lstResults.Items.Add("   ✅ Excellent vulnerability management program!");
                    lstResults.Items.Add("   • Continue regular vulnerability assessments");
                    lstResults.Items.Add("   • Implement threat intelligence feeds");
                    lstResults.Items.Add("   • Consider penetration testing");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lstResults.Items.Add("   ⚠️ Vulnerability management needs enhancement:");
                    lstResults.Items.Add("   • Implement automated vulnerability scanning");
                    lstResults.Items.Add("   • Establish patch management procedures");
                    lstResults.Items.Add("   • Regular CVE monitoring for critical systems");
                    lstResults.Items.Add("   • Create vulnerability remediation timeline");
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lstResults.Items.Add("   🚨 CRITICAL: Vulnerability management gaps detected:");
                    lstResults.Items.Add("   • URGENT: Scan for critical vulnerabilities immediately");
                    lstResults.Items.Add("   • Prioritize patching of internet-facing systems");
                    lstResults.Items.Add("   • Implement emergency patch procedures");
                    lstResults.Items.Add("   • Consider taking vulnerable systems offline");
                    lstResults.Items.Add("   • Establish incident response procedures");
                    break;
            }
            
            lstResults.Items.Add("   📚 Best Practices:");
            lstResults.Items.Add("   • Monthly vulnerability scans at minimum");
            lstResults.Items.Add("   • Risk-based vulnerability prioritization");
            lstResults.Items.Add("   • Integration with patch management systems");
            lstResults.Items.Add("   • Regular security awareness training");
        }

        private void UpdateOverallScoreDisplay(int score, Scans.Audit.DataClasses.ComplianceLevel level)
        {
            // Update the large score value
            lblScoreValue.Text = score.ToString();
            
            // Set color based on compliance level using enhanced dark mode colors
            switch (level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    lblScoreValue.ForeColor = DarkModeTheme.StatusSuccess;
                    lblScoreStatus.Text = "🟢 Good Compliance";
                    lblScoreStatus.ForeColor = DarkModeTheme.StatusSuccess;
                    break;
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    lblScoreValue.ForeColor = DarkModeTheme.StatusWarning;
                    lblScoreStatus.Text = "🟡 Needs Improvement";
                    lblScoreStatus.ForeColor = DarkModeTheme.StatusWarning;
                    break;
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    lblScoreValue.ForeColor = DarkModeTheme.StatusError;
                    lblScoreStatus.Text = "🔴 Critical Issues";
                    lblScoreStatus.ForeColor = DarkModeTheme.StatusError;
                    break;
                default:
                    lblScoreValue.ForeColor = DarkModeTheme.TextMuted;
                    lblScoreStatus.Text = "Not Scanned";
                    lblScoreStatus.ForeColor = DarkModeTheme.TextMuted;
                    break;
            }
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Check workflow prerequisites
            if (!isAssessmentCompleted)
            {
                MessageBox.Show("Please complete the IT Infrastructure Assessment first.", 
                              "Assessment Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!isScanCompleted || currentAuditResult == null)
            {
                MessageBox.Show("Please run a security audit first before generating a report.", 
                              "Scan Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Extract detailed category suggestions
            reportData.NetworkSecuritySuggestions = ExtractCategorySuggestions(auditResult.NetworkSecurity);
            reportData.SystemSecuritySuggestions = ExtractCategorySuggestions(auditResult.SystemSecurity);
            reportData.VulnerabilityManagementSuggestions = ExtractCategorySuggestions(auditResult.VulnerabilityManagement);
            reportData.PasswordSecuritySuggestions = ExtractCategorySuggestions(auditResult.PasswordSecurity);
            reportData.WebSecuritySuggestions = ExtractCategorySuggestions(auditResult.WebSecurity);

            // Include assessment data if available
            if (currentAssessmentResult != null)
            {
                reportData.Assessment = currentAssessmentResult;
                // Debug: Add to key findings to verify data is being included
                reportData.KeyFindings.Add($"DEBUG: Assessment data included - Grade: {currentAssessmentResult.SecurityGrade}, Score: {currentAssessmentResult.SecurityScore}");
            }
            else
            {
                // Debug: Indicate no assessment data
                reportData.KeyFindings.Add("DEBUG: No assessment data available - complete IT Infrastructure Assessment first");
            }

            return reportData;
        }

        private LLM.DataClasses.CategorySuggestions ExtractCategorySuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            // Generate suggestions based on category type and compliance level
            switch (category.Name.ToLower())
            {
                case "password security":
                    suggestions = GeneratePasswordSecuritySuggestions(category);
                    break;
                case "web security":
                    suggestions = GenerateWebSecuritySuggestions(category);
                    break;
                case "network security":
                    suggestions = GenerateNetworkSecuritySuggestions(category);
                    break;
                case "system security":
                    suggestions = GenerateSystemSecuritySuggestions(category);
                    break;
                case "vulnerability management":
                    suggestions = GenerateVulnerabilityManagementSuggestions(category);
                    break;
                default:
                    suggestions.SummaryMessage = $"{category.Name} scored {category.Score}/100";
                    break;
            }

            return suggestions;
        }

        private LLM.DataClasses.CategorySuggestions GeneratePasswordSecuritySuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    suggestions.SummaryMessage = "Excellent password security posture!";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Consider implementing passwordless authentication where possible",
                        "Regular security awareness training for users",
                        "Monitor for compromised credentials in data breaches"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    suggestions.SummaryMessage = "Good foundation, but improvements needed";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Enable Multi-Factor Authentication (MFA) for all accounts",
                        "Implement password complexity requirements",
                        "Set up account lockout policies",
                        "Regular password expiration (90-180 days)"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                default: // Handle any edge cases including score of 0
                    suggestions.SummaryMessage = "CRITICAL: Immediate password security improvements required";
                    suggestions.CriticalActions.AddRange(new[]
                    {
                        "URGENT: Enable Multi-Factor Authentication immediately",
                        "Enforce minimum 12-character passwords",
                        "Require uppercase, lowercase, numbers, and special characters",
                        "Implement account lockout after 5 failed attempts",
                        "Enable password history (remember last 12 passwords)",
                        "Force password changes for all users within 30 days"
                    });
                    break;
            }
            
            suggestions.BestPractices.AddRange(new[]
            {
                "Use a reputable password manager",
                "Never reuse passwords across different systems",
                "Regular security awareness training",
                "Monitor for suspicious login activities"
            });

            return suggestions;
        }

        private LLM.DataClasses.CategorySuggestions GenerateWebSecuritySuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    suggestions.SummaryMessage = "Strong web security implementation!";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Consider implementing Content Security Policy (CSP)",
                        "Regular security header audits",
                        "Monitor for new web vulnerabilities"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    suggestions.SummaryMessage = "Web security needs attention";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Ensure all traffic uses HTTPS with valid certificates",
                        "Implement security headers (HSTS, X-Frame-Options)",
                        "Regular TLS configuration reviews"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    suggestions.SummaryMessage = "CRITICAL: Web security vulnerabilities detected";
                    suggestions.CriticalActions.AddRange(new[]
                    {
                        "URGENT: Enable HTTPS for all web applications",
                        "Install valid SSL/TLS certificates",
                        "Configure security headers immediately",
                        "Disable weak TLS protocols (TLS 1.0, 1.1)"
                    });
                    break;
            }

            return suggestions;
        }

        private LLM.DataClasses.CategorySuggestions GenerateNetworkSecuritySuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    suggestions.SummaryMessage = "Strong network security posture!";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Continue monitoring for new threats",
                        "Regular firewall rule reviews",
                        "Consider network segmentation improvements"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    suggestions.SummaryMessage = "Network security improvements needed";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Close unnecessary open ports",
                        "Implement proper firewall rules",
                        "Regular network vulnerability scanning"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    suggestions.SummaryMessage = "CRITICAL: Network security issues found";
                    suggestions.CriticalActions.AddRange(new[]
                    {
                        "URGENT: Close high-risk open ports",
                        "Implement network firewall immediately",
                        "Disable unnecessary network services",
                        "Enable network intrusion detection"
                    });
                    break;
            }

            return suggestions;
        }

        private LLM.DataClasses.CategorySuggestions GenerateSystemSecuritySuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    suggestions.SummaryMessage = "Strong system security configuration!";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Continue regular system monitoring",
                        "Implement advanced threat detection",
                        "Regular security baseline reviews"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    suggestions.SummaryMessage = "System security needs improvements";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Enable automatic security updates",
                        "Implement endpoint detection and response (EDR)",
                        "Regular system hardening reviews",
                        "Configure centralized logging"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    suggestions.SummaryMessage = "CRITICAL: System security vulnerabilities found";
                    suggestions.CriticalActions.AddRange(new[]
                    {
                        "URGENT: Install security patches immediately",
                        "Enable Windows Defender or equivalent antivirus",
                        "Disable unnecessary system services",
                        "Configure Windows Firewall",
                        "Enable User Account Control (UAC)"
                    });
                    break;
            }
            
            suggestions.BestPractices.AddRange(new[]
            {
                "Regular system updates and patches",
                "Principle of least privilege for user accounts",
                "Regular security scanning and monitoring",
                "Backup and disaster recovery planning"
            });

            return suggestions;
        }

        private LLM.DataClasses.CategorySuggestions GenerateVulnerabilityManagementSuggestions(Scans.Audit.DataClasses.SecurityCategory category)
        {
            var suggestions = new LLM.DataClasses.CategorySuggestions
            {
                CategoryName = category.Name,
                Score = category.Score,
                ComplianceLevel = category.Level.GetText()
            };

            switch (category.Level)
            {
                case Scans.Audit.DataClasses.ComplianceLevel.Green:
                    suggestions.SummaryMessage = "Excellent vulnerability management program!";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Continue regular vulnerability assessments",
                        "Implement threat intelligence feeds",
                        "Consider penetration testing"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Yellow:
                    suggestions.SummaryMessage = "Vulnerability management needs enhancement";
                    suggestions.ImmediateSuggestions.AddRange(new[]
                    {
                        "Implement automated vulnerability scanning",
                        "Establish patch management procedures",
                        "Regular CVE monitoring for critical systems",
                        "Create vulnerability remediation timeline"
                    });
                    break;
                    
                case Scans.Audit.DataClasses.ComplianceLevel.Red:
                    suggestions.SummaryMessage = "CRITICAL: Vulnerability management gaps detected";
                    suggestions.CriticalActions.AddRange(new[]
                    {
                        "URGENT: Scan for critical vulnerabilities immediately",
                        "Prioritize patching of internet-facing systems",
                        "Implement emergency patch procedures",
                        "Consider taking vulnerable systems offline",
                        "Establish incident response procedures"
                    });
                    break;
            }
            
            suggestions.BestPractices.AddRange(new[]
            {
                "Monthly vulnerability scans at minimum",
                "Risk-based vulnerability prioritization",
                "Integration with patch management systems",
                "Regular security awareness training"
            });

            return suggestions;
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
                var result = quizForm.ShowDialog();
                
                // Store the assessment result for potential report generation
                if (quizForm.CompletedAssessment != null)
                {
                    currentAssessmentResult = quizForm.CompletedAssessment;
                    isAssessmentCompleted = true;
                    UpdateWorkflowState();
                    
                    MessageBox.Show($"Assessment completed! Grade: {currentAssessmentResult.SecurityGrade}, Score: {currentAssessmentResult.SecurityScore}.\n\nYou can now proceed to run the security audit.", 
                                  "Assessment Complete - Next Step Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Assessment was not completed properly. Please try again.", 
                                  "Assessment Issue", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening assessment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            try
            {
                // Ask user if they want to reset the entire workflow or just clear scan results
                var result = MessageBox.Show(
                    "Choose what to clear:\n\n" +
                    "• Click 'Yes' to clear only scan results (keep assessment data)\n" +
                    "• Click 'No' to reset entire workflow (clear assessment + scan data)\n" +
                    "• Click 'Cancel' to keep everything",
                    "Clear Options", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                
                // Clear the results listbox
                lstResults.Items.Clear();
                
                // Reset status
                lblStatus.Text = "Ready";
                
                // Reset progress bar
                progressBar.Value = 0;
                SetProgressBarColor(System.Drawing.Color.Gray);
                
                
                // Reset overall score display
                lblScoreValue.Text = "--";
                lblScoreValue.ForeColor = System.Drawing.Color.Gray;
                lblScoreStatus.Text = "No Scan Performed";
                lblScoreStatus.ForeColor = System.Drawing.Color.Gray;
                
                // Clear current audit result
                currentAuditResult = null;
                isScanCompleted = false;
                
                if (result == DialogResult.No)
                {
                    // Reset entire workflow
                    currentAssessmentResult = null;
                    isAssessmentCompleted = false;
                    lblStatus.Text = "Workflow reset - start with IT Infrastructure Assessment";
                }
                else
                {
                    // Keep assessment, just clear scan results
                    lblStatus.Text = "Scan results cleared - assessment data preserved";
                }
                
                UpdateWorkflowState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing results: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
