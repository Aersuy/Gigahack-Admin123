using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LLM.DataClasses;

namespace Gigahack_Admin123
{
    public partial class QuizHistoryForm : Form
    {
        private List<QuizSession> quizHistory;

        public QuizHistoryForm(List<QuizSession> history)
        {
            InitializeComponent();
            quizHistory = history;
            DisplayHistory();
        }

        private void DisplayHistory()
        {
            lstHistory.Items.Clear();
            
            if (!quizHistory.Any())
            {
                lstHistory.Items.Add("No assessment history available.");
                return;
            }

            lstHistory.Items.Add("=== ASSESSMENT HISTORY ===");
            lstHistory.Items.Add("");
            
            var sortedHistory = quizHistory.OrderByDescending(h => h.StartTime).ToList();
            
            for (int i = 0; i < sortedHistory.Count; i++)
            {
                var session = sortedHistory[i];
                var duration = session.EndTime.HasValue ? 
                    (session.EndTime.Value - session.StartTime).ToString(@"mm\:ss") : "Incomplete";
                
                lstHistory.Items.Add($"Assessment #{i + 1}");
                lstHistory.Items.Add($"Date: {session.StartTime:yyyy-MM-dd HH:mm:ss}");
                lstHistory.Items.Add($"Score: {session.Score}/{session.TotalQuestions} ({session.PercentageScore:F1}%)");
                lstHistory.Items.Add($"Grade: {GetGrade(session.PercentageScore)}");
                lstHistory.Items.Add($"Duration: {duration}");
                lstHistory.Items.Add($"Status: {(session.IsCompleted ? "Completed" : "Incomplete")}");
                lstHistory.Items.Add("");
            }
            
            // Add statistics
            if (sortedHistory.Any(s => s.IsCompleted))
            {
                var completedQuizzes = sortedHistory.Where(s => s.IsCompleted).ToList();
                var averageScore = completedQuizzes.Average(s => s.PercentageScore);
                var bestScore = completedQuizzes.Max(s => s.PercentageScore);
                var totalAssessments = completedQuizzes.Count;
                
                lstHistory.Items.Add("=== STATISTICS ===");
                lstHistory.Items.Add("");
                lstHistory.Items.Add($"Total Completed Assessments: {totalAssessments}");
                lstHistory.Items.Add($"Average Score: {averageScore:F1}%");
                lstHistory.Items.Add($"Best Score: {bestScore:F1}%");
                lstHistory.Items.Add($"Best Grade: {GetGrade(bestScore)}");
            }
        }

        private string GetGrade(double percentage)
        {
            return percentage switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C", 
                >= 60 => "D",
                _ => "F"
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all assessment history?", 
                                       "Clear History", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (System.IO.File.Exists("quiz_sessions.json"))
                    {
                        System.IO.File.Delete("quiz_sessions.json");
                        MessageBox.Show("Assessment history cleared successfully!", "Success", 
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                        quizHistory.Clear();
                        DisplayHistory();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error clearing history: {ex.Message}", "Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
