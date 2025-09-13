namespace Gigahack_Admin123
{
    partial class QuizForm
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
            lblTitle = new Label();
            lblQuestionNumber = new Label();
            lblCategory = new Label();
            lblQuestion = new Label();
            panelOptions = new Panel();
            btnNext = new Button();
            btnStartOver = new Button();
            btnViewHistory = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(22, 19);
            lblTitle.Margin = new Padding(6, 0, 6, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(673, 59);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📋 IT Infrastructure Assessment";
            // 
            // lblQuestionNumber
            // 
            lblQuestionNumber.AutoSize = true;
            lblQuestionNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblQuestionNumber.Location = new Point(22, 128);
            lblQuestionNumber.Margin = new Padding(6, 0, 6, 0);
            lblQuestionNumber.Name = "lblQuestionNumber";
            lblQuestionNumber.Size = new Size(249, 45);
            lblQuestionNumber.TabIndex = 1;
            lblQuestionNumber.Text = "Question 1 of 5";
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            lblCategory.Location = new Point(22, 192);
            lblCategory.Margin = new Padding(6, 0, 6, 0);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(333, 37);
            lblCategory.TabIndex = 2;
            lblCategory.Text = "Category: Network Security";
            lblCategory.Click += lblCategory_Click;
            // 
            // lblQuestion
            // 
            lblQuestion.Font = new Font("Segoe UI", 12F);
            lblQuestion.Location = new Point(22, 256);
            lblQuestion.Margin = new Padding(6, 0, 6, 0);
            lblQuestion.Name = "lblQuestion";
            lblQuestion.Size = new Size(1040, 128);
            lblQuestion.TabIndex = 3;
            lblQuestion.Text = "What does HTTPS stand for?";
            // 
            // panelOptions
            // 
            panelOptions.BackColor = SystemColors.Control;
            panelOptions.BorderStyle = BorderStyle.FixedSingle;
            panelOptions.Location = new Point(22, 405);
            panelOptions.Margin = new Padding(6, 6, 6, 6);
            panelOptions.Name = "panelOptions";
            panelOptions.Size = new Size(1038, 318);
            panelOptions.TabIndex = 4;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.FromArgb(0, 120, 215);
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(509, 766);
            btnNext.Margin = new Padding(6, 6, 6, 6);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(186, 75);
            btnNext.TabIndex = 5;
            btnNext.Text = "Next Question";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // btnStartOver
            // 
            btnStartOver.BackColor = Color.FromArgb(220, 53, 69);
            btnStartOver.FlatAppearance.BorderSize = 0;
            btnStartOver.FlatStyle = FlatStyle.Flat;
            btnStartOver.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStartOver.ForeColor = Color.White;
            btnStartOver.Location = new Point(22, 768);
            btnStartOver.Margin = new Padding(6, 6, 6, 6);
            btnStartOver.Name = "btnStartOver";
            btnStartOver.Size = new Size(186, 75);
            btnStartOver.TabIndex = 6;
            btnStartOver.Text = "Start Over";
            btnStartOver.UseVisualStyleBackColor = false;
            btnStartOver.Click += btnStartOver_Click;
            // 
            // btnViewHistory
            // 
            btnViewHistory.BackColor = Color.FromArgb(138, 43, 226);
            btnViewHistory.FlatAppearance.BorderSize = 0;
            btnViewHistory.FlatStyle = FlatStyle.Flat;
            btnViewHistory.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnViewHistory.ForeColor = Color.White;
            btnViewHistory.Location = new Point(262, 766);
            btnViewHistory.Margin = new Padding(6, 6, 6, 6);
            btnViewHistory.Name = "btnViewHistory";
            btnViewHistory.Size = new Size(186, 75);
            btnViewHistory.TabIndex = 7;
            btnViewHistory.Text = "View History";
            btnViewHistory.UseVisualStyleBackColor = false;
            btnViewHistory.Click += btnViewHistory_Click;
            // 
            // QuizForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1085, 877);
            Controls.Add(btnViewHistory);
            Controls.Add(btnStartOver);
            Controls.Add(btnNext);
            Controls.Add(panelOptions);
            Controls.Add(lblQuestion);
            Controls.Add(lblCategory);
            Controls.Add(lblQuestionNumber);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6, 6, 6, 6);
            MaximizeBox = false;
            Name = "QuizForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "IT Infrastructure Assessment";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblQuestionNumber;
        private Label lblCategory;
        private Label lblQuestion;
        private Panel panelOptions;
        private Button btnNext;
        private Button btnStartOver;
        private Button btnViewHistory;
    }
}
