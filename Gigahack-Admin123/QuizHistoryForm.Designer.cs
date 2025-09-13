namespace Gigahack_Admin123
{
    partial class QuizHistoryForm
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
            this.lstHistory = new ListBox();
            this.btnClose = new Button();
            this.btnClearHistory = new Button();
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
            this.lblTitle.Text = "📊 Assessment History";
            // 
            // lstHistory
            // 
            this.lstHistory.Font = new Font("Consolas", 9F);
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.ItemHeight = 14;
            this.lstHistory.Location = new Point(12, 50);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new Size(560, 350);
            this.lstHistory.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Location = new Point(497, 410);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(75, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Location = new Point(12, 410);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new Size(100, 30);
            this.btnClearHistory.TabIndex = 3;
            this.btnClearHistory.Text = "Clear History";
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += new EventHandler(this.btnClearHistory_Click);
            // 
            // QuizHistoryForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(584, 452);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lstHistory);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "QuizHistoryForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Assessment History";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private ListBox lstHistory;
        private Button btnClose;
        private Button btnClearHistory;
    }
}
