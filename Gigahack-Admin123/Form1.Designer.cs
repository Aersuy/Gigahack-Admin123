namespace Gigahack_Admin123
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblTargetIP = new Label();
            this.txtTargetIP = new TextBox();
            this.btnScan = new Button();
            this.btnScanLocalhost = new Button();
            this.btnScanCommon = new Button();
            this.btnEmailAuth = new Button();
            this.btnStop = new Button();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.lstResults = new ListBox();
            this.lblResults = new Label();
            this.lblOpenPorts = new Label();
            this.lblClosedPorts = new Label();
            this.numMaxPorts = new NumericUpDown();
            this.lblMaxPorts = new Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(200, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Gigahack Port Scanner";
            // 
            // lblTargetIP
            // 
            this.lblTargetIP.AutoSize = true;
            this.lblTargetIP.Location = new Point(12, 50);
            this.lblTargetIP.Name = "lblTargetIP";
            this.lblTargetIP.Size = new Size(58, 15);
            this.lblTargetIP.TabIndex = 1;
            this.lblTargetIP.Text = "Target IP:";
            // 
            // txtTargetIP
            // 
            this.txtTargetIP.Location = new Point(76, 47);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new Size(150, 23);
            this.txtTargetIP.TabIndex = 2;
            this.txtTargetIP.Text = "127.0.0.1";
            // 
            // lblMaxPorts
            // 
            this.lblMaxPorts.AutoSize = true;
            this.lblMaxPorts.Location = new Point(250, 50);
            this.lblMaxPorts.Name = "lblMaxPorts";
            this.lblMaxPorts.Size = new Size(70, 15);
            this.lblMaxPorts.TabIndex = 3;
            this.lblMaxPorts.Text = "Max Ports:";
            // 
            // numMaxPorts
            // 
            this.numMaxPorts.Location = new Point(326, 47);
            this.numMaxPorts.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numMaxPorts.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numMaxPorts.Name = "numMaxPorts";
            this.numMaxPorts.Size = new Size(80, 23);
            this.numMaxPorts.TabIndex = 4;
            this.numMaxPorts.Value = new decimal(new int[] { 500, 0, 0, 0 });
            // 
            // btnScan
            // 
            this.btnScan.Location = new Point(12, 85);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new Size(75, 30);
            this.btnScan.TabIndex = 5;
            this.btnScan.Text = "Start Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new EventHandler(this.btnScan_Click);
            // 
            // btnScanLocalhost
            // 
            this.btnScanLocalhost.Location = new Point(93, 85);
            this.btnScanLocalhost.Name = "btnScanLocalhost";
            this.btnScanLocalhost.Size = new Size(100, 30);
            this.btnScanLocalhost.TabIndex = 6;
            this.btnScanLocalhost.Text = "Scan Localhost";
            this.btnScanLocalhost.UseVisualStyleBackColor = true;
            this.btnScanLocalhost.Click += new EventHandler(this.btnScanLocalhost_Click);
            // 
            // btnScanCommon
            // 
            this.btnScanCommon.Location = new Point(199, 85);
            this.btnScanCommon.Name = "btnScanCommon";
            this.btnScanCommon.Size = new Size(100, 30);
            this.btnScanCommon.TabIndex = 7;
            this.btnScanCommon.Text = "Scan Common";
            this.btnScanCommon.UseVisualStyleBackColor = true;
            this.btnScanCommon.Click += new EventHandler(this.btnScanCommon_Click);
            // 
            // btnEmailAuth
            // 
            this.btnEmailAuth.Location = new Point(305, 85);
            this.btnEmailAuth.Name = "btnEmailAuth";
            this.btnEmailAuth.Size = new Size(100, 30);
            this.btnEmailAuth.TabIndex = 8;
            this.btnEmailAuth.Text = "Email Auth";
            this.btnEmailAuth.UseVisualStyleBackColor = true;
            this.btnEmailAuth.Click += new EventHandler(this.btnEmailAuth_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new Point(411, 85);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(75, 30);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop Scan";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new EventHandler(this.btnStop_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(12, 130);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(776, 23);
            this.progressBar.TabIndex = 7;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(12, 160);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(42, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Ready";
            // 
            // lstResults
            // 
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 15;
            this.lstResults.Location = new Point(12, 200);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new Size(776, 199);
            this.lstResults.TabIndex = 9;
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new Point(12, 182);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new Size(47, 15);
            this.lblResults.TabIndex = 10;
            this.lblResults.Text = "Results:";
            // 
            // lblOpenPorts
            // 
            this.lblOpenPorts.AutoSize = true;
            this.lblOpenPorts.ForeColor = Color.Green;
            this.lblOpenPorts.Location = new Point(600, 182);
            this.lblOpenPorts.Name = "lblOpenPorts";
            this.lblOpenPorts.Size = new Size(75, 15);
            this.lblOpenPorts.TabIndex = 11;
            this.lblOpenPorts.Text = "Open: 0";
            // 
            // lblClosedPorts
            // 
            this.lblClosedPorts.AutoSize = true;
            this.lblClosedPorts.ForeColor = Color.Red;
            this.lblClosedPorts.Location = new Point(700, 182);
            this.lblClosedPorts.Name = "lblClosedPorts";
            this.lblClosedPorts.Size = new Size(80, 15);
            this.lblClosedPorts.TabIndex = 12;
            this.lblClosedPorts.Text = "Closed: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 420);
            this.Controls.Add(this.lblClosedPorts);
            this.Controls.Add(this.lblOpenPorts);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnEmailAuth);
            this.Controls.Add(this.btnScanCommon);
            this.Controls.Add(this.btnScanLocalhost);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.numMaxPorts);
            this.Controls.Add(this.lblMaxPorts);
            this.Controls.Add(this.txtTargetIP);
            this.Controls.Add(this.lblTargetIP);
            this.Controls.Add(this.lblTitle);
            this.Name = "Form1";
            this.Text = "Gigahack Port Scanner";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblTargetIP;
        private TextBox txtTargetIP;
        private Button btnScan;
        private Button btnScanLocalhost;
        private Button btnScanCommon;
        private Button btnEmailAuth;
        private Button btnStop;
        private ProgressBar progressBar;
        private Label lblStatus;
        private ListBox lstResults;
        private Label lblResults;
        private Label lblOpenPorts;
        private Label lblClosedPorts;
        private NumericUpDown numMaxPorts;
        private Label lblMaxPorts;
    }
}
