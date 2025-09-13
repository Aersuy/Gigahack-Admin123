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
            this.components = new System.ComponentModel.Container();
            
            // Initialize all controls
            this.lblTitle = new Label();
            this.lblTargetIP = new Label();
            this.txtTargetIP = new TextBox();
            this.btnAuditDashboard = new Button();
            this.btnStop = new Button();
            this.btnGenerateReport = new Button();
            this.btnQuiz = new Button();
            this.btnClearResults = new Button();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.lstResults = new ListBox();
            this.lblResults = new Label();
            this.lblOpenPorts = new Label();
            this.lblClosedPorts = new Label();
            this.lblOverallScore = new Label();
            this.lblScoreValue = new Label();
            this.lblScoreStatus = new Label();
            
            this.SuspendLayout();
            // =============================================================================
            // HEADER SECTION
            // =============================================================================
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblTitle.ForeColor = Color.FromArgb(64, 64, 64);
            this.lblTitle.Location = new Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(350, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔍 Security Audit Dashboard";
            
            // =============================================================================
            // INPUT SECTION
            // =============================================================================
            
            // lblTargetIP
            this.lblTargetIP.AutoSize = true;
            this.lblTargetIP.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblTargetIP.ForeColor = Color.FromArgb(80, 80, 80);
            this.lblTargetIP.Location = new Point(20, 70);
            this.lblTargetIP.Name = "lblTargetIP";
            this.lblTargetIP.Size = new Size(120, 20);
            this.lblTargetIP.TabIndex = 1;
            this.lblTargetIP.Text = "Target IP/Domain:";
            
            // txtTargetIP
            this.txtTargetIP.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.txtTargetIP.Location = new Point(150, 68);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new Size(200, 25);
            this.txtTargetIP.TabIndex = 2;
            this.txtTargetIP.Text = "127.0.0.1";
            this.txtTargetIP.BorderStyle = BorderStyle.FixedSingle;
            // =============================================================================
            // CONTROL BUTTONS SECTION
            // =============================================================================
            
            // btnAuditDashboard - Primary Action Button
            this.btnAuditDashboard.BackColor = Color.FromArgb(0, 120, 215);
            this.btnAuditDashboard.FlatAppearance.BorderSize = 0;
            this.btnAuditDashboard.FlatStyle = FlatStyle.Flat;
            this.btnAuditDashboard.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnAuditDashboard.ForeColor = Color.White;
            this.btnAuditDashboard.Location = new Point(20, 110);
            this.btnAuditDashboard.Name = "btnAuditDashboard";
            this.btnAuditDashboard.Size = new Size(180, 45);
            this.btnAuditDashboard.TabIndex = 3;
            this.btnAuditDashboard.Text = "🔍 Run Security Audit";
            this.btnAuditDashboard.UseVisualStyleBackColor = false;
            this.btnAuditDashboard.Click += new EventHandler(this.btnAuditDashboard_Click);
            this.btnAuditDashboard.MouseEnter += (s, e) => { this.btnAuditDashboard.BackColor = Color.FromArgb(0, 100, 180); };
            this.btnAuditDashboard.MouseLeave += (s, e) => { this.btnAuditDashboard.BackColor = Color.FromArgb(0, 120, 215); };
            
            // btnStop - Secondary Action Button
            this.btnStop.BackColor = Color.FromArgb(196, 43, 28);
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = FlatStyle.Flat;
            this.btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnStop.ForeColor = Color.White;
            this.btnStop.Location = new Point(210, 110);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(80, 45);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "⏹ Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new EventHandler(this.btnStop_Click);
            
            // btnGenerateReport - Utility Button
            this.btnGenerateReport.BackColor = Color.FromArgb(16, 137, 62);
            this.btnGenerateReport.FlatAppearance.BorderSize = 0;
            this.btnGenerateReport.FlatStyle = FlatStyle.Flat;
            this.btnGenerateReport.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnGenerateReport.ForeColor = Color.White;
            this.btnGenerateReport.Location = new Point(300, 110);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new Size(140, 45);
            this.btnGenerateReport.TabIndex = 5;
            this.btnGenerateReport.Text = "📄 Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = false;
            this.btnGenerateReport.Click += new EventHandler(this.btnGenerateReport_Click);
            this.btnGenerateReport.MouseEnter += (s, e) => { this.btnGenerateReport.BackColor = Color.FromArgb(14, 120, 55); };
            this.btnGenerateReport.MouseLeave += (s, e) => { this.btnGenerateReport.BackColor = Color.FromArgb(16, 137, 62); };
            
            // btnQuiz - Assessment Button
            this.btnQuiz.BackColor = Color.FromArgb(138, 43, 226);
            this.btnQuiz.FlatAppearance.BorderSize = 0;
            this.btnQuiz.FlatStyle = FlatStyle.Flat;
            this.btnQuiz.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnQuiz.ForeColor = Color.White;
            this.btnQuiz.Location = new Point(450, 110);
            this.btnQuiz.Name = "btnQuiz";
            this.btnQuiz.Size = new Size(130, 45);
            this.btnQuiz.TabIndex = 6;
            this.btnQuiz.Text = "📋 IT Assessment";
            this.btnQuiz.UseVisualStyleBackColor = false;
            this.btnQuiz.Click += new EventHandler(this.btnQuiz_Click);
            this.btnQuiz.MouseEnter += (s, e) => { this.btnQuiz.BackColor = Color.FromArgb(120, 40, 200); };
            this.btnQuiz.MouseLeave += (s, e) => { this.btnQuiz.BackColor = Color.FromArgb(138, 43, 226); };
            
            // btnClearResults
            this.btnClearResults.BackColor = Color.FromArgb(220, 53, 69);
            this.btnClearResults.FlatAppearance.BorderSize = 0;
            this.btnClearResults.FlatStyle = FlatStyle.Flat;
            this.btnClearResults.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnClearResults.ForeColor = Color.White;
            this.btnClearResults.Location = new Point(570, 110);
            this.btnClearResults.Name = "btnClearResults";
            this.btnClearResults.Size = new Size(100, 45);
            this.btnClearResults.TabIndex = 7;
            this.btnClearResults.Text = "🗑️ Clear";
            this.btnClearResults.UseVisualStyleBackColor = false;
            this.btnClearResults.Click += new EventHandler(this.btnClearResults_Click);
            this.btnClearResults.MouseEnter += (s, e) => { this.btnClearResults.BackColor = Color.FromArgb(200, 35, 51); };
            this.btnClearResults.MouseLeave += (s, e) => { this.btnClearResults.BackColor = Color.FromArgb(220, 53, 69); };
            
            // =============================================================================
            // PROGRESS AND STATUS SECTION
            // =============================================================================
            
            // progressBar
            this.progressBar.Location = new Point(20, 170);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(760, 12);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 7;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            this.progressBar.Value = 0;
            
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblStatus.Location = new Point(20, 190);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(45, 19);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Ready";
            // =============================================================================
            // RESULTS SECTION
            // =============================================================================
            
            // lblResults
            this.lblResults.AutoSize = true;
            this.lblResults.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblResults.ForeColor = Color.FromArgb(64, 64, 64);
            this.lblResults.Location = new Point(20, 220);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new Size(65, 21);
            this.lblResults.TabIndex = 9;
            this.lblResults.Text = "Results:";
            
            // Port Status Labels
            this.lblOpenPorts.AutoSize = true;
            this.lblOpenPorts.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblOpenPorts.ForeColor = Color.FromArgb(16, 137, 62);
            this.lblOpenPorts.Location = new Point(600, 222);
            this.lblOpenPorts.Name = "lblOpenPorts";
            this.lblOpenPorts.Size = new Size(65, 19);
            this.lblOpenPorts.TabIndex = 10;
            this.lblOpenPorts.Text = "Open: 0";
            
            this.lblClosedPorts.AutoSize = true;
            this.lblClosedPorts.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblClosedPorts.ForeColor = Color.FromArgb(196, 43, 28);
            this.lblClosedPorts.Location = new Point(680, 222);
            this.lblClosedPorts.Name = "lblClosedPorts";
            this.lblClosedPorts.Size = new Size(75, 19);
            this.lblClosedPorts.TabIndex = 11;
            this.lblClosedPorts.Text = "Closed: 0";
            
            // lstResults
            this.lstResults.BackColor = Color.FromArgb(250, 250, 250);
            this.lstResults.BorderStyle = BorderStyle.FixedSingle;
            this.lstResults.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 14;
            this.lstResults.Location = new Point(20, 250);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new Size(760, 170);
            this.lstResults.TabIndex = 12;
            // =============================================================================
            // OVERALL SCORE SECTION
            // =============================================================================
            
            // lblOverallScore
            this.lblOverallScore.AutoSize = true;
            this.lblOverallScore.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblOverallScore.ForeColor = Color.FromArgb(64, 64, 64);
            this.lblOverallScore.Location = new Point(20, 440);
            this.lblOverallScore.Name = "lblOverallScore";
            this.lblOverallScore.Size = new Size(125, 25);
            this.lblOverallScore.TabIndex = 13;
            this.lblOverallScore.Text = "Overall Score:";
            
            // lblScoreValue - Large prominent score display
            this.lblScoreValue.AutoSize = true;
            this.lblScoreValue.Font = new Font("Segoe UI", 52F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblScoreValue.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblScoreValue.Location = new Point(160, 415);
            this.lblScoreValue.Name = "lblScoreValue";
            this.lblScoreValue.Size = new Size(124, 94);
            this.lblScoreValue.TabIndex = 14;
            this.lblScoreValue.Text = "---";
            this.lblScoreValue.TextAlign = ContentAlignment.MiddleCenter;
            
            // lblScoreStatus - Status text with modern styling
            this.lblScoreStatus.AutoSize = true;
            this.lblScoreStatus.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblScoreStatus.ForeColor = Color.FromArgb(100, 100, 100);
            this.lblScoreStatus.Location = new Point(300, 450);
            this.lblScoreStatus.Name = "lblScoreStatus";
            this.lblScoreStatus.Size = new Size(134, 30);
            this.lblScoreStatus.TabIndex = 15;
            this.lblScoreStatus.Text = "Not Scanned";
            this.lblScoreStatus.TextAlign = ContentAlignment.MiddleLeft;
            // =============================================================================
            // FORM CONFIGURATION
            // =============================================================================
            
            // Form1
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.ClientSize = new Size(800, 530);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Form1";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "🔍 Security Audit Dashboard - Professional Edition";
            
            // Add controls in organized order
            this.Controls.AddRange(new Control[] {
                // Header Section
                this.lblTitle,
                
                // Input Section  
                this.lblTargetIP,
                this.txtTargetIP,
                
                // Button Section
                this.btnAuditDashboard,
                this.btnStop,
                this.btnGenerateReport,
                this.btnQuiz,
                this.btnClearResults,
                
                // Progress Section
                this.progressBar,
                this.lblStatus,
                
                // Results Section
                this.lblResults,
                this.lblOpenPorts,
                this.lblClosedPorts,
                this.lstResults,
                
                // Score Section
                this.lblOverallScore,
                this.lblScoreValue,
                this.lblScoreStatus
            });
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblTargetIP;
        private TextBox txtTargetIP;
        private Button btnAuditDashboard;
        private Button btnStop;
        private Button btnGenerateReport;
        private Button btnQuiz;
        private Button btnClearResults;
        private ProgressBar progressBar;
        private Label lblStatus;
        private ListBox lstResults;
        private Label lblResults;
        private Label lblOpenPorts;
        private Label lblClosedPorts;
        private Label lblOverallScore;
        private Label lblScoreValue;
        private Label lblScoreStatus;
    }
}
