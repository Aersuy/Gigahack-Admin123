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
            
            // Subscribe to port scanner events
            portScanner.PortScanned += OnPortScanned;
            portScanner.ProgressUpdated += OnProgressUpdated;
            portScanner.StatusChanged += OnStatusChanged;
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            await StartScanAsync();
        }

        // Event handlers for port scanner events
        private void OnPortScanned(int port, bool isOpen)
        {
            // Update UI on the main thread
            this.Invoke(new Action(() =>
            {
                if (isOpen)
                {
                    lstResults.Items.Add($"Port {port} - OPEN");
                    openPortsCount++;
                }
                else
                {
                    closedPortsCount++;
                }
                UpdatePortCounts();
            }));
        }

        private void OnProgressUpdated(int completed, int total)
        {
            // Update progress bar on the main thread
            this.Invoke(new Action(() =>
            {
                progressBar.Value = completed;
            }));
        }

        private void OnStatusChanged(string status)
        {
            // Update status label on the main thread
            this.Invoke(new Action(() =>
            {
                lblStatus.Text = status;
            }));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            lblStatus.Text = "Stopping scan...";
        }

        private void UpdatePortCounts()
        {
            lblOpenPorts.Text = $"Open: {openPortsCount}";
            lblClosedPorts.Text = $"Closed: {closedPortsCount}";
        }

        private async void btnScanLocalhost_Click(object sender, EventArgs e)
        {
            // Set localhost IP and start scan
            txtTargetIP.Text = "127.0.0.1";
            await StartScanAsync();
        }

        private async Task StartScanAsync()
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
            btnStop.Enabled = true;
            txtTargetIP.Enabled = false;
            numMaxPorts.Enabled = false;

            // Create cancellation token
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await portScanner.ScanWithProgressAsync(txtTargetIP.Text, (int)numMaxPorts.Value, 1000, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Status will be updated by the event handler
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error occurred";
            }
            finally
            {
                // Reset UI state
                btnScan.Enabled = true;
                btnScanLocalhost.Enabled = true;
                btnStop.Enabled = false;
                txtTargetIP.Enabled = true;
                numMaxPorts.Enabled = true;
                progressBar.Value = progressBar.Maximum;
            }
        }
    }
}
