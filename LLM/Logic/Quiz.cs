
using LLM.DataClasses;
using System.Text.Json;
using System.IO;

namespace LLM.Logic
{
    public class Quiz
    {
        private List<QuizQuestion> questions;
        private const string QuizDataFile = "quiz_sessions.json";
        
        public Quiz()
        {
            InitializeQuestions();
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Id = 1,
                    Question = "How many employees work in your organization?",
                    Options = new List<string> { "1-10 employees", "11-50 employees", "51-200 employees", "200+ employees" },
                    CorrectAnswerIndex = -1, // No correct answer for assessment
                    Category = "Organization Size",
                    Explanation = "This helps determine the scale of your IT infrastructure needs and security requirements."
                },
                new QuizQuestion
                {
                    Id = 2,
                    Question = "What type of network infrastructure does your organization primarily use?",
                    Options = new List<string> { "On-premises servers and local network", "Cloud-based infrastructure (AWS, Azure, Google Cloud)", "Hybrid (mix of on-premises and cloud)", "Mostly remote/distributed workforce" },
                    CorrectAnswerIndex = -1,
                    Category = "Network Infrastructure",
                    Explanation = "Understanding your network setup helps identify potential security vulnerabilities and infrastructure gaps."
                },
                new QuizQuestion
                {
                    Id = 3,
                    Question = "How often do you perform security updates and patches?",
                    Options = new List<string> { "Automatically as they're released", "Monthly scheduled updates", "Quarterly or less frequent", "Only when critical issues arise" },
                    CorrectAnswerIndex = -1,
                    Category = "Patch Management",
                    Explanation = "Regular patching is crucial for maintaining security and preventing exploitation of known vulnerabilities."
                },
                new QuizQuestion
                {
                    Id = 4,
                    Question = "What backup strategy does your organization use?",
                    Options = new List<string> { "Daily automated backups with offsite storage", "Weekly backups to local storage", "Monthly or irregular backups", "No formal backup strategy" },
                    CorrectAnswerIndex = -1,
                    Category = "Data Backup",
                    Explanation = "Proper backup strategies are essential for business continuity and disaster recovery."
                },
                new QuizQuestion
                {
                    Id = 5,
                    Question = "How do employees access company systems remotely?",
                    Options = new List<string> { "VPN with multi-factor authentication", "VPN with password only", "Direct internet access to systems", "No remote access capabilities" },
                    CorrectAnswerIndex = -1,
                    Category = "Remote Access",
                    Explanation = "Secure remote access is critical for protecting company resources and data."
                },
                new QuizQuestion
                {
                    Id = 6,
                    Question = "What antivirus/endpoint protection solution do you use?",
                    Options = new List<string> { "Enterprise endpoint detection and response (EDR)", "Traditional antivirus software", "Built-in Windows/Mac protection only", "No antivirus protection" },
                    CorrectAnswerIndex = -1,
                    Category = "Endpoint Security",
                    Explanation = "Modern endpoint protection is essential for detecting and preventing malware and advanced threats."
                },
                new QuizQuestion
                {
                    Id = 7,
                    Question = "How is sensitive data (customer info, financial data) protected?",
                    Options = new List<string> { "Encrypted at rest and in transit with access controls", "Basic password protection", "Stored on shared drives with limited access", "No special protection measures" },
                    CorrectAnswerIndex = -1,
                    Category = "Data Protection",
                    Explanation = "Proper data protection is crucial for compliance and preventing data breaches."
                },
                new QuizQuestion
                {
                    Id = 8,
                    Question = "What email security measures are in place?",
                    Options = new List<string> { "Advanced email security with spam/phishing protection", "Basic spam filtering", "Standard email client protection", "No additional email security" },
                    CorrectAnswerIndex = -1,
                    Category = "Email Security",
                    Explanation = "Email is a common attack vector and requires robust security measures."
                },
                new QuizQuestion
                {
                    Id = 9,
                    Question = "How often do you conduct security awareness training?",
                    Options = new List<string> { "Quarterly with phishing simulations", "Annual training sessions", "Informal training as needed", "No formal security training" },
                    CorrectAnswerIndex = -1,
                    Category = "Security Training",
                    Explanation = "Regular security training helps employees recognize and avoid security threats."
                },
                new QuizQuestion
                {
                    Id = 10,
                    Question = "Do you have an incident response plan for security breaches?",
                    Options = new List<string> { "Documented plan with regular testing", "Basic plan but not regularly updated", "Informal response procedures", "No incident response plan" },
                    CorrectAnswerIndex = -1,
                    Category = "Incident Response",
                    Explanation = "Having a proper incident response plan is crucial for minimizing damage from security incidents."
                },
                new QuizQuestion
                {
                    Id = 11,
                    Question = "What type of firewall protection do you have?",
                    Options = new List<string> { "Next-generation firewall with intrusion detection", "Traditional firewall", "Router-based firewall only", "No dedicated firewall" },
                    CorrectAnswerIndex = -1,
                    Category = "Network Security",
                    Explanation = "Proper firewall protection is the first line of defense against network threats."
                },
                new QuizQuestion
                {
                    Id = 12,
                    Question = "How do you manage user access and permissions?",
                    Options = new List<string> { "Role-based access control with regular reviews", "Basic user groups and permissions", "Individual permission assignments", "All users have similar access levels" },
                    CorrectAnswerIndex = -1,
                    Category = "Access Control",
                    Explanation = "Proper access control ensures users only have access to resources they need for their job."
                }
            };
        }

        public List<QuizQuestion> GetRandomQuestions(int count = 5)
        {
            var random = new Random();
            return questions.OrderBy(x => random.Next()).Take(count).ToList();
        }

        public List<QuizQuestion> GetQuestionsByCategory(string category)
        {
            return questions.Where(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<string> GetCategories()
        {
            return questions.Select(q => q.Category).Distinct().ToList();
        }

        public QuizResult CreateQuizSession(List<QuizQuestion> quizQuestions)
        {
            return new QuizResult
            {
                Session = new QuizSession
                {
                    TotalQuestions = quizQuestions.Count
                },
                Questions = quizQuestions
            };
        }

        public void SubmitAnswer(QuizResult quizResult, int questionId, int selectedAnswerIndex)
        {
            var question = quizResult.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question != null)
            {
                // For assessment, calculate security score based on answer quality
                var securityScore = CalculateSecurityScore(question, selectedAnswerIndex);
                
                var answer = new QuizAnswer
                {
                    QuestionId = questionId,
                    SelectedAnswerIndex = selectedAnswerIndex,
                    IsCorrect = securityScore > 2 // Consider high security scores as "correct"
                };

                quizResult.Session.Answers.Add(answer);
                quizResult.Session.Score += securityScore;
            }
        }

        private int CalculateSecurityScore(QuizQuestion question, int selectedAnswerIndex)
        {
            // Security scoring based on best practices
            // Options are generally ordered from most secure (index 0) to least secure (index 3)
            return selectedAnswerIndex switch
            {
                0 => 4, // Most secure option
                1 => 3, // Good security
                2 => 2, // Basic security
                3 => 1, // Poor security
                _ => 0  // No answer
            };
        }

        public void CompleteQuiz(QuizResult quizResult)
        {
            quizResult.Session.EndTime = DateTime.Now;
            quizResult.Session.IsCompleted = true;
            SaveQuizSession(quizResult.Session);
        }

        private void SaveQuizSession(QuizSession session)
        {
            try
            {
                List<QuizSession> sessions = new List<QuizSession>();
                
                if (File.Exists(QuizDataFile))
                {
                    var existingData = File.ReadAllText(QuizDataFile);
                    if (!string.IsNullOrEmpty(existingData))
                    {
                        sessions = JsonSerializer.Deserialize<List<QuizSession>>(existingData) ?? new List<QuizSession>();
                    }
                }

                sessions.Add(session);
                
                var json = JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(QuizDataFile, json);
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                Console.WriteLine($"Error saving quiz session: {ex.Message}");
            }
        }

        public List<QuizSession> GetQuizHistory()
        {
            try
            {
                if (File.Exists(QuizDataFile))
                {
                    var json = File.ReadAllText(QuizDataFile);
                    return JsonSerializer.Deserialize<List<QuizSession>>(json) ?? new List<QuizSession>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading quiz history: {ex.Message}");
            }
            
            return new List<QuizSession>();
        }
    }
}
