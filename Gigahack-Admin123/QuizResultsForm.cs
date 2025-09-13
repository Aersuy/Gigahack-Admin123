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
    public partial class QuizResultsForm : Form
    {
        private QuizResult quizResult;
        private List<QuizQuestion> questions;

        public QuizResultsForm(QuizResult result, List<QuizQuestion> quizQuestions)
        {
            InitializeComponent();
            quizResult = result;
            questions = quizQuestions;
            DisplayResults();
        }

        private void DisplayResults()
        {
            // Display overall security score
            var maxScore = quizResult.Session.TotalQuestions * 4;
            lblScore.Text = $"Security Score: {quizResult.Session.Score}/{maxScore}";
            lblPercentage.Text = $"Security Level: {quizResult.Session.PercentageScore:F1}%";
            
            var securityLevel = quizResult.Session.PercentageScore switch
            {
                >= 80 => "Excellent",
                >= 65 => "Good", 
                >= 50 => "Fair",
                >= 35 => "Poor",
                _ => "Critical"
            };
            
            lblGrade.Text = $"Assessment: {securityLevel}";
            
            // Set color based on security level (Green-Yellow-Red scheme)
            var gradeColor = quizResult.Session.PercentageScore switch
            {
                >= 75 => Color.Green,        // Green for good scores (75%+)
                >= 50 => Color.Orange,       // Yellow/Orange for medium scores (50-74%)
                >= 25 => Color.Red,          // Red for poor scores (25-49%)
                _ => Color.DarkRed           // Dark red for critical scores (<25%)
            };
            
            lblScore.ForeColor = gradeColor;
            lblPercentage.ForeColor = gradeColor;
            lblGrade.ForeColor = gradeColor;
            
            // Display detailed assessment results
            lstResults.Items.Clear();
            lstResults.Items.Add("=== IT INFRASTRUCTURE ASSESSMENT DETAILS ===");
            lstResults.Items.Add("");
            
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var answer = quizResult.Session.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                
                lstResults.Items.Add($"Area {i + 1}: {question.Question}");
                lstResults.Items.Add($"Category: {question.Category}");
                
                if (answer != null)
                {
                    var selectedOption = answer.SelectedAnswerIndex < question.Options.Count ? 
                        question.Options[answer.SelectedAnswerIndex] : "No answer provided";
                    
                    // Calculate security score for this answer (Green-Yellow-Red scheme)
                    var securityScore = answer.SelectedAnswerIndex switch
                    {
                        0 => "🟢 Excellent Security",
                        1 => "🟢 Good Security", 
                        2 => "🟡 Basic Security",
                        3 => "🔴 Poor Security",
                        _ => "❌ No Response"
                    };
                    
                    lstResults.Items.Add($"Your Response: {selectedOption}");
                    lstResults.Items.Add($"Security Assessment: {securityScore}");
                    lstResults.Items.Add($"Recommendation: {question.Explanation}");
                }
                else
                {
                    lstResults.Items.Add("Your Response: No answer provided");
                    lstResults.Items.Add("Security Assessment: ❌ No Response");
                    lstResults.Items.Add($"Recommendation: {question.Explanation}");
                }
                
                lstResults.Items.Add("");
            }
            
            // Add security assessment feedback
            lstResults.Items.Add("=== SECURITY ASSESSMENT SUMMARY ===");
            lstResults.Items.Add("");
            
            var feedback = quizResult.Session.PercentageScore switch
            {
                >= 80 => "Excellent! Your organization has strong IT security practices in place. Continue monitoring and maintaining these high standards.",
                >= 65 => "Good! Your IT infrastructure has solid security foundations. Consider addressing the areas marked for improvement.",
                >= 50 => "Fair. Your organization has basic security measures, but there are several areas that need attention to improve your security posture.",
                >= 35 => "Poor. Your IT infrastructure has significant security gaps that should be addressed immediately to prevent potential breaches.",
                _ => "Critical! Your organization's IT security requires immediate attention. Consider consulting with cybersecurity professionals."
            };
            
            lstResults.Items.Add(feedback);
            
            // Add category performance
            var categoryPerformance = new Dictionary<string, (int correct, int total)>();
            
            foreach (var question in questions)
            {
                var answer = quizResult.Session.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                
                if (!categoryPerformance.ContainsKey(question.Category))
                {
                    categoryPerformance[question.Category] = (0, 0);
                }
                
                var (correct, total) = categoryPerformance[question.Category];
                categoryPerformance[question.Category] = (
                    correct + (answer?.IsCorrect == true ? 1 : 0),
                    total + 1
                );
            }
            
            if (categoryPerformance.Any())
            {
                lstResults.Items.Add("");
                lstResults.Items.Add("=== CATEGORY PERFORMANCE ===");
                lstResults.Items.Add("");
                
                foreach (var kvp in categoryPerformance)
                {
                    var percentage = (double)kvp.Value.correct / kvp.Value.total * 100;
                    lstResults.Items.Add($"{kvp.Key}: {kvp.Value.correct}/{kvp.Value.total} ({percentage:F1}%)");
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTakeAnother_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
