using Scans.InternetExposure;
using System.Net;

namespace Gigahack_Admin123
{
    public partial class Form1 : Form
    {
        private PortScanner portScanner;
        private CancellationTokenSource? cancellationTokenSource;
        private int openPortsCount = 0;
        private int closedPortsCount = 0;

        public Form1()
        {
            InitializeComponent();
            portScanner = new PortScanner();
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
    }
}
