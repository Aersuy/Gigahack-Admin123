using Scans.InternetExposure.ScanLogic;
using Scans.InternetExposure.DataClases;
using Scans.CVE.ScanLogic;
using Scans.CVE.DataClasses;
using Scans.Password.PasswordLogic;
using Scans.Password.DataClasses;
using System.Net;

namespace Gigahack_Admin123
{
    public partial class Form1 : Form
    {
        private PortScanner portScanner;
        private EmailAuthScanner emailAuthScanner;
        private CVEScanner cveScanner;
        private PasswordChangeScan passwordPolicyScanner;
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
            
            // Add cleanup on form close
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            cveScanner?.Dispose();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            StartScan();
        }

        private void btnScanLocalhost_Click(object sender, EventArgs e)
        {
            // Set localhost IP and start scan
            txtTargetIP.Text = "127.0.0.1";
            StartScan();
        }

        private void btnScanCommon_Click(object sender, EventArgs e)
        {
            StartCommonPortScan();
        }

        private void StartScan()
        {
            if (string.IsNullOrWhiteSpace(txtTargetIP.Text))
            {
                MessageBox.Show("Please enter a target IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IPAddress.TryParse(txtTargetIP.Text, out _))
            {
                MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reset UI
            lstResults.Items.Clear();
            openPortsCount = 0;
            closedPortsCount = 0;
            UpdatePortCounts();
            progressBar.Value = 0;
            progressBar.Maximum = (int)numMaxPorts.Value;

            // Update UI state
            btnScan.Enabled = false;
            btnScanLocalhost.Enabled = false;
            btnScanCommon.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Scanning...";
            txtTargetIP.Enabled = false;
            numMaxPorts.Enabled = false;

            // Run scan on background thread to prevent UI freezing
            Task.Run(() => PerformScan(txtTargetIP.Text, (int)numMaxPorts.Value));
        }

        private void StartCommonPortScan()
        {
            if (string.IsNullOrWhiteSpace(txtTargetIP.Text))
            {
                MessageBox.Show("Please enter a target IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IPAddress.TryParse(txtTargetIP.Text, out _))
            {
                MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Reset UI
            lstResults.Items.Clear();
            openPortsCount = 0;
            closedPortsCount = 0;
            UpdatePortCounts();
            progressBar.Value = 0;
            progressBar.Maximum = 14; // 14 common ports

            // Update UI state
            btnScan.Enabled = false;
            btnScanLocalhost.Enabled = false;
            btnScanCommon.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Scanning common ports...";
            txtTargetIP.Enabled = false;
            numMaxPorts.Enabled = false;

            // Run scan on background thread to prevent UI freezing
            Task.Run(() => PerformCommonPortScan(txtTargetIP.Text));
        }

        private void PerformScan(string ipAddress, int maxPorts)
        {
            try
            {
                // Call your synchronous AllPortScan method
                var results = portScanner.AllPortScan(ipAddress);
                
                // Update UI on main thread
                this.Invoke(new Action(() =>
                {
                    foreach (var result in results)
                    {
                        if (result.open)
                        {
                            string displayText = $"Port {result.portNumber} ({result.service}) - OPEN";
                            if (!string.IsNullOrEmpty(result.banner))
                            {
                                displayText += $" - {result.banner.Trim()}";
                            }
                            lstResults.Items.Add(displayText);
                            openPortsCount++;
                        }
                        else
                        {
                            closedPortsCount++;
                        }
                        
                        // Update progress
                        progressBar.Value++;
                        UpdatePortCounts();
                    }
                    
                    lblStatus.Text = "Scan completed";
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Error occurred";
                }));
            }
            finally
            {
                // Reset UI state on main thread
                this.Invoke(new Action(() =>
                {
                    btnScan.Enabled = true;
                    btnScanLocalhost.Enabled = true;
                    btnScanCommon.Enabled = true;
                    btnStop.Enabled = false;
                    txtTargetIP.Enabled = true;
                    numMaxPorts.Enabled = true;
                    progressBar.Value = progressBar.Maximum;
                }));
            }
        }

        private void PerformCommonPortScan(string ipAddress)
        {
            try
            {
                // Call your synchronous ScanCommonPorts method
                var results = portScanner.ScanCommonPorts(ipAddress);
                
                // Update UI on main thread
                this.Invoke(new Action(() =>
                {
                    foreach (var result in results)
                    {
                        if (result.open)
                        {
                            string displayText = $"Port {result.portNumber} ({result.service}) - OPEN";
                            if (!string.IsNullOrEmpty(result.banner))
                            {
                                displayText += $" - {result.banner.Trim()}";
                            }
                            lstResults.Items.Add(displayText);
                            openPortsCount++;
                        }
                        else
                        {
                            closedPortsCount++;
                        }
                        
                        // Update progress
                        progressBar.Value++;
                        UpdatePortCounts();
                    }
                    
                    lblStatus.Text = "Common ports scan completed";
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Error occurred";
                }));
            }
            finally
            {
                // Reset UI state on main thread
                this.Invoke(new Action(() =>
                {
                    btnScan.Enabled = true;
                    btnScanLocalhost.Enabled = true;
                    btnScanCommon.Enabled = true;
                    btnStop.Enabled = false;
                    txtTargetIP.Enabled = true;
                    numMaxPorts.Enabled = true;
                    progressBar.Value = progressBar.Maximum;
                }));
            }
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

        // Email Authentication Methods
        private void btnEmailAuth_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTargetIP.Text))
            {
                MessageBox.Show("Please enter a domain to scan", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnEmailAuth.Enabled = false;
            lblStatus.Text = "Scanning email authentication...";

            try
            {
                // Run on background thread to prevent UI freezing
                Task.Run(() => {
                    var result = emailAuthScanner.ScanEmailAuth(txtTargetIP.Text);
                    
                    // Update UI on main thread
                    this.Invoke(new Action(() => {
                        DisplayEmailAuthResults(result);
                        lblStatus.Text = "Email authentication scan completed";
                    }));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Email auth scan error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Email auth scan failed";
            }
            finally
            {
                btnEmailAuth.Enabled = true;
            }
        }

        private void DisplayEmailAuthResults(EmailAuthResult result)
        {
            lstResults.Items.Clear();
            
            lstResults.Items.Add($"=== Email Authentication Report for {result.Domain} ===");
            lstResults.Items.Add($"Security Score: {result.SecurityScore}/100 (Grade: {result.SecurityGrade})");
            lstResults.Items.Add($"Scan Time: {result.ScanTime:yyyy-MM-dd HH:mm:ss}");
            lstResults.Items.Add("");
            
            // SPF Results
            lstResults.Items.Add("=== SPF (Sender Policy Framework) ===");
            lstResults.Items.Add($"Record Exists: {result.SPF.RecordExists}");
            if (result.SPF.RecordExists)
            {
                lstResults.Items.Add($"Record: {result.SPF.Record}");
                lstResults.Items.Add($"Valid: {result.SPF.IsValid}");
                lstResults.Items.Add($"Qualifier: {result.SPF.Qualifier}");
                if (result.SPF.Mechanisms.Any())
                    lstResults.Items.Add($"Mechanisms: {string.Join(", ", result.SPF.Mechanisms)}");
                if (result.SPF.Includes.Any())
                    lstResults.Items.Add($"Includes: {string.Join(", ", result.SPF.Includes)}");
            }
            else
            {
                lstResults.Items.Add("No SPF record found");
            }
            lstResults.Items.Add("");
            
            // DKIM Results
            lstResults.Items.Add("=== DKIM (DomainKeys Identified Mail) ===");
            if (result.DKIM.Any())
            {
                foreach (var dkim in result.DKIM)
                {
                    lstResults.Items.Add($"Selector: {dkim.Selector}");
                    lstResults.Items.Add($"Valid: {dkim.IsValid}");
                    lstResults.Items.Add($"Key Length: {dkim.KeyLength} bits");
                    lstResults.Items.Add($"Key Type: {dkim.KeyType}");
                    if (!string.IsNullOrEmpty(dkim.Algorithm))
                        lstResults.Items.Add($"Algorithm: {dkim.Algorithm}");
                    if (!string.IsNullOrEmpty(dkim.HashAlgorithm))
                        lstResults.Items.Add($"Hash Algorithm: {dkim.HashAlgorithm}");
                    if (!string.IsNullOrEmpty(dkim.PublicKey))
                    {
                        lstResults.Items.Add($"Public Key: {dkim.PublicKey}");
                        // Show truncated version for display
                        var truncatedKey = dkim.PublicKey.Length > 50 ? 
                            dkim.PublicKey.Substring(0, 50) + "..." : 
                            dkim.PublicKey;
                        lstResults.Items.Add($"Public Key (truncated): {truncatedKey}");
                    }
                    lstResults.Items.Add("");
                }
            }
            else
            {
                lstResults.Items.Add("No DKIM records found");
            }
            lstResults.Items.Add("");
            
            // DMARC Results
            lstResults.Items.Add("=== DMARC (Domain-based Message Authentication) ===");
            lstResults.Items.Add($"Record Exists: {result.DMARC.RecordExists}");
            if (result.DMARC.RecordExists)
            {
                lstResults.Items.Add($"Record: {result.DMARC.Record}");
                lstResults.Items.Add($"Valid: {result.DMARC.IsValid}");
                lstResults.Items.Add($"Policy: {result.DMARC.Policy}");
                lstResults.Items.Add($"Percentage: {result.DMARC.Percentage}%");
                if (!string.IsNullOrEmpty(result.DMARC.RUA))
                    lstResults.Items.Add($"Aggregate Reports: {result.DMARC.RUA}");
                if (!string.IsNullOrEmpty(result.DMARC.RUF))
                    lstResults.Items.Add($"Forensic Reports: {result.DMARC.RUF}");
            }
            else
            {
                lstResults.Items.Add("No DMARC record found");
            }
            lstResults.Items.Add("");
            
            // Warnings and Errors
            var allWarnings = result.SPF.Warnings.Concat(result.DMARC.Warnings)
                .Concat(result.DKIM.SelectMany(d => d.Warnings)).ToList();
            var allErrors = result.SPF.Errors.Concat(result.DMARC.Errors)
                .Concat(result.DKIM.SelectMany(d => d.Errors)).ToList();
            
            if (allWarnings.Any())
            {
                lstResults.Items.Add("=== Warnings ===");
                foreach (var warning in allWarnings)
                {
                    lstResults.Items.Add($"⚠️ {warning}");
                }
                lstResults.Items.Add("");
            }
            
            if (allErrors.Any())
            {
                lstResults.Items.Add("=== Errors ===");
                foreach (var error in allErrors)
                {
                    lstResults.Items.Add($"❌ {error}");
                }
            }
        }

        // CVE Search Methods
        private void btnCVE_Click(object sender, EventArgs e)
        {
            if (lstResults.Items.Count == 0)
            {
                MessageBox.Show("Please run a port scan first to search for CVEs based on discovered services.", "No Scan Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnCVE.Enabled = false;
            lblStatus.Text = "Searching for CVEs...";

            try
            {
                // Run on background thread to prevent UI freezing
                Task.Run(async () => {
                    try
                    {
                        // First test the APIs
                        await cveScanner.TestAPIs();
                        
                        // Get the port scan results from the current display
                        var portResults = GetPortScanResultsFromDisplay();
                        var result = await cveScanner.SearchCVEsForPortScanResults(portResults);
                        
                        // Update UI on main thread
                        this.Invoke(new Action(() => {
                            DisplayCVEResults(result);
                            lblStatus.Text = "CVE search completed";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => {
                            MessageBox.Show($"CVE search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "CVE search failed";
                        }));
                    }
                    finally
                    {
                        this.Invoke(new Action(() => {
                            btnCVE.Enabled = true;
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CVE search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "CVE search failed";
                btnCVE.Enabled = true;
            }
        }

        private List<PortScanResult> GetPortScanResultsFromDisplay()
        {
            var portItems = new List<PortScanItem>();
            
            foreach (var item in lstResults.Items)
            {
                var itemText = item.ToString();
                if (itemText != null && itemText.Contains(" - OPEN"))
                {
                    // Parse the displayed port information
                    // Format: "Port 80 (http) - OPEN - Apache/2.4.41"
                    var parts = itemText.Split(" - ");
                    if (parts.Length >= 2)
                    {
                        var portPart = parts[0]; // "Port 80 (http)"
                        var statusPart = parts[1]; // "OPEN"
                        
                        if (statusPart == "OPEN")
                        {
                            // Extract port number and service
                            var portMatch = System.Text.RegularExpressions.Regex.Match(portPart, @"Port (\d+) \((.+)\)");
                            if (portMatch.Success)
                            {
                                var portNumber = int.Parse(portMatch.Groups[1].Value);
                                var service = portMatch.Groups[2].Value;
                                
                                // Extract banner if available
                                var banner = parts.Length > 2 ? string.Join(" - ", parts.Skip(2)) : "";
                                
                                portItems.Add(new PortScanItem
                                {
                                    portNumber = portNumber,
                                    service = service,
                                    banner = banner,
                                    open = true
                                });
                            }
                        }
                    }
                }
            }
            
            // Create a single PortScanResult containing all the port items
            return new List<PortScanResult> 
            { 
                new PortScanResult 
                { 
                    Ports = portItems 
                } 
            };
        }

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

        // Password Policy Methods
        private void btnPasswordPolicy_Click(object sender, EventArgs e)
        {
            btnPasswordPolicy.Enabled = false;
            lblStatus.Text = "Scanning password policies...";

            try
            {
                // Run on background thread to prevent UI freezing
                Task.Run(async () => {
                    try
                    {
                        // Use the target IP as domain controller if provided, otherwise scan local machine
                        var domainController = !string.IsNullOrEmpty(txtTargetIP.Text) && txtTargetIP.Text != "127.0.0.1" ? txtTargetIP.Text : null;
                        var result = await passwordPolicyScanner.ScanPasswordPolicies(domainController);
                        
                        // Update UI on main thread
                        this.Invoke(new Action(() => {
                            DisplayPasswordPolicyResults(result);
                            lblStatus.Text = "Password policy scan completed";
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => {
                            MessageBox.Show($"Password policy scan error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            lblStatus.Text = "Password policy scan failed";
                        }));
                    }
                    finally
                    {
                        this.Invoke(new Action(() => {
                            btnPasswordPolicy.Enabled = true;
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Password policy scan error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Password policy scan failed";
                btnPasswordPolicy.Enabled = true;
            }
        }

        private void DisplayPasswordPolicyResults(PasswordPolicyResult result)
        {
            lstResults.Items.Clear();
            
            lstResults.Items.Add($"=== Password Policy Security Report ===");
            lstResults.Items.Add($"Target: {result.Target}");
            lstResults.Items.Add($"Security Score: {result.SecurityScore}/100 (Grade: {result.SecurityGrade})");
            lstResults.Items.Add($"Scan Time: {result.ScanTime:yyyy-MM-dd HH:mm:ss}");
            lstResults.Items.Add("");

            if (!result.Success)
            {
                lstResults.Items.Add($"❌ Error: {result.ErrorMessage}");
                return;
            }

            // Password Complexity
            lstResults.Items.Add("=== Password Complexity ===");
            lstResults.Items.Add($"Enabled: {result.Complexity.Enabled}");
            if (result.Complexity.Enabled)
            {
                lstResults.Items.Add($"Minimum Length: {result.Complexity.MinimumLength} characters");
                lstResults.Items.Add($"Require Uppercase: {result.Complexity.RequireUppercase}");
                lstResults.Items.Add($"Require Lowercase: {result.Complexity.RequireLowercase}");
                lstResults.Items.Add($"Require Numbers: {result.Complexity.RequireNumbers}");
                lstResults.Items.Add($"Require Special Characters: {result.Complexity.RequireSpecialCharacters}");
                lstResults.Items.Add($"Minimum Character Sets: {result.Complexity.MinimumCharacterSets}");
            }
            else
            {
                lstResults.Items.Add("❌ Password complexity is disabled");
            }
            lstResults.Items.Add("");

            // Password History
            lstResults.Items.Add("=== Password History ===");
            lstResults.Items.Add($"Enabled: {result.History.Enabled}");
            if (result.History.Enabled)
            {
                lstResults.Items.Add($"Remembered Passwords: {result.History.RememberedPasswords}");
                lstResults.Items.Add($"Enforced: {result.History.Enforced}");
            }
            else
            {
                lstResults.Items.Add("❌ Password history is disabled");
            }
            lstResults.Items.Add("");

            // Password Age
            lstResults.Items.Add("=== Password Age Policy ===");
            lstResults.Items.Add($"Enabled: {result.Age.Enabled}");
            if (result.Age.Enabled)
            {
                lstResults.Items.Add($"Maximum Age: {result.Age.MaximumAge} days");
                lstResults.Items.Add($"Minimum Age: {result.Age.MinimumAge} days");
                lstResults.Items.Add($"Enforced: {result.Age.Enforced}");
            }
            else
            {
                lstResults.Items.Add("❌ Password age policy is disabled");
            }
            lstResults.Items.Add("");

            // Account Lockout
            lstResults.Items.Add("=== Account Lockout Policy ===");
            lstResults.Items.Add($"Enabled: {result.Lockout.Enabled}");
            if (result.Lockout.Enabled)
            {
                lstResults.Items.Add($"Lockout Threshold: {result.Lockout.LockoutThreshold} attempts");
                lstResults.Items.Add($"Lockout Duration: {result.Lockout.LockoutDuration} minutes");
                lstResults.Items.Add($"Reset Count After: {result.Lockout.ResetCountAfter} minutes");
                lstResults.Items.Add($"Enforced: {result.Lockout.Enforced}");
            }
            else
            {
                lstResults.Items.Add("❌ Account lockout policy is disabled");
            }
            lstResults.Items.Add("");

            // Password Change Frequency
            lstResults.Items.Add("=== Password Change Frequency ===");
            lstResults.Items.Add($"Enabled: {result.ChangeFrequency.Enabled}");
            if (result.ChangeFrequency.Enabled)
            {
                lstResults.Items.Add($"Minimum Days Between Changes: {result.ChangeFrequency.MinimumDaysBetweenChanges}");
                lstResults.Items.Add($"Enforced: {result.ChangeFrequency.Enforced}");
            }
            else
            {
                lstResults.Items.Add("❌ Password change frequency policy is disabled");
            }
            lstResults.Items.Add("");

            // Recommendations
            if (result.Recommendations.Any())
            {
                lstResults.Items.Add("=== Security Recommendations ===");
                foreach (var recommendation in result.Recommendations)
                {
                    lstResults.Items.Add($"💡 {recommendation}");
                }
                lstResults.Items.Add("");
            }

            // Warnings
            if (result.Warnings.Any())
            {
                lstResults.Items.Add("=== Warnings ===");
                foreach (var warning in result.Warnings)
                {
                    lstResults.Items.Add($"⚠️ {warning}");
                }
                lstResults.Items.Add("");
            }

            // Errors
            if (result.Errors.Any())
            {
                lstResults.Items.Add("=== Errors ===");
                foreach (var error in result.Errors)
                {
                    lstResults.Items.Add($"❌ {error}");
                }
            }
        }

    }
}
