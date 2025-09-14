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
            
            // Apply Windows 11 Dark Mode Theme AFTER everything is initialized
            this.Load += QuizResultsForm_Load;
        }

        private void QuizResultsForm_Load(object sender, EventArgs e)
        {
            // Apply dark mode theme after the form is fully loaded
            ApplyDarkModeTheme();
        }

        private void DisplayResults()
        {
            // Check for incident-related answers and show warning if needed
            CheckForIncidentWarning();
            
            // Check for poor quiz scores and show training recommendations
            CheckForTrainingRecommendations();
            
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

        private void CheckForIncidentWarning()
        {
            // Check if user answered "Yes" to question 24 (cybersecurity incidents in last 12 months)
            var incidentAnswer = quizResult.Session.Answers.FirstOrDefault(a => a.QuestionId == 24);
            
            if (incidentAnswer != null)
            {
                // Find question 24 to get the answer text
                var incidentQuestion = questions.FirstOrDefault(q => q.Id == 24);
                if (incidentQuestion != null && incidentAnswer.SelectedAnswerIndex < incidentQuestion.Options.Count)
                {
                    var selectedAnswer = incidentQuestion.Options[incidentAnswer.SelectedAnswerIndex];
                    
                    // Show warning if user answered "Yes" to having incidents
                    if (selectedAnswer.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowIncidentReportingWarning();
                    }
                }
            }
        }

        private void ShowIncidentReportingWarning()
        {
            var warningMessage = "⚠️ IMPORTANT LEGAL NOTICE ⚠️\n\n" +
                               "Based on your assessment responses indicating recent cybersecurity incidents, " +
                               "please be aware that:\n\n" +
                               "• Many jurisdictions require organizations to report cybersecurity incidents to relevant authorities\n" +
                               "• Reporting timeframes are typically within 24 hours of incident discovery\n" +
                               "• Failure to report may result in regulatory penalties\n\n" +
                               "RECOMMENDED ACTIONS:\n" +
                               "• Contact your legal counsel immediately\n" +
                               "• Review applicable regulations\n" +
                               "• Prepare incident documentation\n" +
                               "• Contact relevant authorities if required\n\n" +
                               "This tool does not provide legal advice. Consult with qualified legal professionals " +
                               "regarding your specific reporting obligations.";

            MessageBox.Show(warningMessage, 
                          "Incident Reporting Requirements", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Warning);
        }

        private void CheckForTrainingRecommendations()
        {
            // Show training recommendations if score is below 50% (Poor or Critical)
            if (quizResult.Session.PercentageScore < 50)
            {
                ShowTrainingRecommendations();
            }
        }

        private void ShowTrainingRecommendations()
        {
            var severityLevel = quizResult.Session.PercentageScore switch
            {
                >= 35 => "Poor",
                _ => "Critical"
            };

            var trainingMessage = $"📚 CYBERSECURITY TRAINING RECOMMENDATIONS 📚\n\n" +
                                $"Your assessment score ({quizResult.Session.PercentageScore:F1}%) indicates {severityLevel.ToLower()} cybersecurity awareness.\n" +
                                "Immediate training is recommended to improve your organization's security posture.\n\n" +
                                "🎯 PRIORITY TRAINING AREAS:\n" +
                                "• Password Security & Multi-Factor Authentication\n" +
                                "• Phishing Recognition & Email Security\n" +
                                "• Safe Web Browsing & Downloads\n" +
                                "• Incident Response Procedures\n" +
                                "• Data Protection & Privacy Compliance\n\n" +
                                "📋 RECOMMENDED ACTIONS:\n" +
                                "• Schedule mandatory cybersecurity training for all staff\n" +
                                "• Implement regular security awareness sessions (monthly)\n" +
                                "• Conduct simulated phishing tests\n" +
                                "• Create security policies and procedures documentation\n" +
                                "• Establish a security champion program\n\n" +
                                "🏢 TRAINING RESOURCES:\n" +
                                "• SANS Security Awareness Training\n" +
                                "• KnowBe4 Security Awareness Platform\n" +
                                "• NIST Cybersecurity Framework Training\n" +
                                "• Industry-specific compliance training (HIPAA, PCI-DSS, etc.)\n" +
                                "• Local cybersecurity training providers\n\n" +
                                "Regular training reduces security risks by up to 70% and helps prevent costly breaches.";

            MessageBox.Show(trainingMessage, 
                          "Cybersecurity Training Recommendations", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Information);
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

        /// <summary>
        /// Apply Windows 11 Dark Mode theme to the quiz results form
        /// </summary>
        private void ApplyDarkModeTheme()
        {
            // Forcefully apply the dark mode theme to override Designer colors
            DarkModeTheme.ForceApplyColors(this);
            
            // Apply rounded corners to buttons with Windows 11 styling
            DarkModeTheme.ApplyRoundedCorners(btnClose, 8);
            DarkModeTheme.ApplyRoundedCorners(btnTakeAnother, 8);
            
            // Apply rounded corners to the results list
            DarkModeTheme.ApplyRoundedCorners(lstResults, 8);
        }
    }
}
