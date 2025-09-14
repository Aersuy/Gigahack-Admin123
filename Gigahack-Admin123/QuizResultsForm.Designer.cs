namespace Gigahack_Admin123
{
    partial class QuizResultsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblScore = new Label();
            this.lblPercentage = new Label();
            this.lblGrade = new Label();
            this.lstResults = new ListBox();
            this.btnClose = new Button();
            this.btnTakeAnother = new Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(180, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 Assessment Results";
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblScore.Location = new Point(12, 60);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new Size(100, 25);
            this.lblScore.TabIndex = 1;
            this.lblScore.Text = "Score: 4/5";
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Font = new Font("Segoe UI", 12F);
            this.lblPercentage.Location = new Point(150, 63);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new Size(120, 21);
            this.lblPercentage.TabIndex = 2;
            this.lblPercentage.Text = "Percentage: 80%";
            // 
            // lblGrade
            // 
            this.lblGrade.AutoSize = true;
            this.lblGrade.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblGrade.Location = new Point(300, 63);
            this.lblGrade.Name = "lblGrade";
            this.lblGrade.Size = new Size(70, 21);
            this.lblGrade.TabIndex = 3;
            this.lblGrade.Text = "Grade: B";
            // 
            // lstResults
            // 
            this.lstResults.BackColor = Color.FromArgb(42, 42, 42);
            this.lstResults.ForeColor = Color.FromArgb(248, 248, 248);
            this.lstResults.Font = new Font("Consolas", 9F);
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 14;
            this.lstResults.Location = new Point(12, 100);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new Size(760, 350);
            this.lstResults.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = Color.FromArgb(55, 55, 55);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(697, 460);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(75, 30);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // btnTakeAnother
            // 
            this.btnTakeAnother.BackColor = Color.FromArgb(60, 60, 60);
            this.btnTakeAnother.FlatAppearance.BorderSize = 0;
            this.btnTakeAnother.FlatStyle = FlatStyle.Flat;
            this.btnTakeAnother.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnTakeAnother.ForeColor = Color.White;
            this.btnTakeAnother.Location = new Point(580, 460);
            this.btnTakeAnother.Name = "btnTakeAnother";
            this.btnTakeAnother.Size = new Size(110, 30);
            this.btnTakeAnother.TabIndex = 6;
            this.btnTakeAnother.Text = "Take Another Assessment";
            this.btnTakeAnother.UseVisualStyleBackColor = false;
            this.btnTakeAnother.Click += new EventHandler(this.btnTakeAnother_Click);
            // 
            // QuizResultsForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.FromArgb(248, 248, 248);
            this.ClientSize = new Size(784, 502);
            this.Controls.Add(this.btnTakeAnother);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.lblGrade);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "QuizResultsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Assessment Results";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblScore;
        private Label lblPercentage;
        private Label lblGrade;
        private ListBox lstResults;
        private Button btnClose;
        private Button btnTakeAnother;
    }
}
