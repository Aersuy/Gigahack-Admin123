using Scans.InternetExposure.ScanLogic;
using Scans.InternetExposure.DataClases;
using System.Net;

namespace Gigahack_Admin123
{
    public partial class Form1 : Form
    {
        private PortScanner portScanner;
        private EmailAuthScanner emailAuthScanner;
        private CancellationTokenSource? cancellationTokenSource;
        private int openPortsCount = 0;
        private int closedPortsCount = 0;

        public Form1()
        {
            InitializeComponent();
            portScanner = new PortScanner();
            emailAuthScanner = new EmailAuthScanner();
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
    }
}
