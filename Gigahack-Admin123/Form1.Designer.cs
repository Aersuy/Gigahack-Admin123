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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.btnAuditDashboard = new Button();
            this.btnStop = new Button();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.lstResults = new ListBox();
            this.lblResults = new Label();
            this.lblOpenPorts = new Label();
            this.lblClosedPorts = new Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(300, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔍 Security Audit Dashboard";
            // 
            // lblTargetIP
            // 
            this.lblTargetIP.AutoSize = true;
            this.lblTargetIP.Location = new Point(12, 50);
            this.lblTargetIP.Name = "lblTargetIP";
            this.lblTargetIP.Size = new Size(100, 15);
            this.lblTargetIP.TabIndex = 1;
            this.lblTargetIP.Text = "Target IP/Domain:";
            // 
            // txtTargetIP
            // 
            this.txtTargetIP.Location = new Point(76, 47);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new Size(150, 23);
            this.txtTargetIP.TabIndex = 2;
            this.txtTargetIP.Text = "127.0.0.1";
            // 
            // btnAuditDashboard
            // 
            this.btnAuditDashboard.Location = new Point(12, 85);
            this.btnAuditDashboard.Name = "btnAuditDashboard";
            this.btnAuditDashboard.Size = new Size(200, 50);
            this.btnAuditDashboard.TabIndex = 5;
            this.btnAuditDashboard.Text = "🔍 Run Security Audit";
            this.btnAuditDashboard.UseVisualStyleBackColor = true;
            this.btnAuditDashboard.Click += new EventHandler(this.btnAuditDashboard_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new Point(220, 85);
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
            this.Controls.Add(this.btnAuditDashboard);
            this.Controls.Add(this.txtTargetIP);
            this.Controls.Add(this.lblTargetIP);
            this.Controls.Add(this.lblTitle);
            this.Name = "Form1";
            this.Text = "Security Audit Dashboard";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblTargetIP;
        private TextBox txtTargetIP;
        private Button btnAuditDashboard;
        private Button btnStop;
        private ProgressBar progressBar;
        private Label lblStatus;
        private ListBox lstResults;
        private Label lblResults;
        private Label lblOpenPorts;
        private Label lblClosedPorts;
    }
}
