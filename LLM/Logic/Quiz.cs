
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
                // Basic Business Profile Questions
                new QuizQuestion
                {
                    Id = 1,
                    Question = "Are you a small\medium enterprise (SME)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Business Profile",
                    Explanation = "SME classification affects compliance requirements and available resources for cybersecurity measures."
                },
                new QuizQuestion
                {
                    Id = 2,
                    Question = "Do you operate in a nationally defined critical sector (e.g., telecom, banking, healthcare, transport, cloud/data center/hosting)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Business Profile",
                    Explanation = "Critical sectors have enhanced cybersecurity requirements and stricter compliance obligations."
                },
                new QuizQuestion
                {
                    Id = 3,
                    Question = "Do you collect or store personal data (customers or employees)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Data Handling",
                    Explanation = "Personal data collection triggers GDPR and other privacy regulation compliance requirements."
                },
                new QuizQuestion
                {
                    Id = 4,
                    Question = "Do you have a website or online application that is important for your business?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Digital Assets",
                    Explanation = "Web applications are common attack vectors and require specific security measures."
                },
                new QuizQuestion
                {
                    Id = 5,
                    Question = "Do you keep business or customer data in a database (including cloud databases or business systems)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Data Storage",
                    Explanation = "Database security is critical for protecting sensitive business and customer information."
                },
                new QuizQuestion
                {
                    Id = 6,
                    Question = "Do you use your own business email domain (e.g., name@yourcompany.md)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Communication",
                    Explanation = "Business email domains require specific security configurations like SPF, DKIM, and DMARC."
                },
                new QuizQuestion
                {
                    Id = 7,
                    Question = "Do you use cloud services (e.g., Microsoft 365, Google Workspace, AWS, Azure)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Cloud Services",
                    Explanation = "Cloud services require specific security configurations and monitoring practices."
                },
                
                // Security Measures Questions
                new QuizQuestion
                {
                    Id = 8,
                    Question = "Is Two-Factor Authentication (2FA/MFA) enabled for administrator or privileged accounts?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Access Control",
                    Explanation = "2FA/MFA significantly reduces the risk of account compromise and unauthorized access."
                },
                new QuizQuestion
                {
                    Id = 9,
                    Question = "Do you keep an inventory of internet-exposed assets (domains, websites, servers, open ports)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Asset Management",
                    Explanation = "Asset inventory is essential for understanding your attack surface and managing security risks."
                },
                new QuizQuestion
                {
                    Id = 10,
                    Question = "Do you regularly apply security updates/patches to systems and applications?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Patch Management",
                    Explanation = "Regular patching is critical for closing security vulnerabilities and preventing exploits."
                },
                new QuizQuestion
                {
                    Id = 11,
                    Question = "Do you centralize security logs (e.g., sign-ins, errors) so you can investigate incidents?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Logging & Monitoring",
                    Explanation = "Centralized logging enables effective incident response and security monitoring."
                },
                
                // Website Security (Conditional)
                new QuizQuestion
                {
                    Id = 12,
                    Question = "Does your website/app enforce HTTPS (TLS) with a valid certificate?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Web Security",
                    Explanation = "HTTPS encryption protects data in transit and is essential for web application security."
                },
                new QuizQuestion
                {
                    Id = 13,
                    Question = "Does your website/app use basic security headers (e.g., HSTS, CSP, X-Frame-Options)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Web Security",
                    Explanation = "Security headers provide additional protection against common web attacks."
                },
                new QuizQuestion
                {
                    Id = 14,
                    Question = "Have you checked TLS configuration and removed weak settings within the last 12 months?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Web Security",
                    Explanation = "Regular TLS configuration reviews ensure strong encryption and remove deprecated protocols."
                },
                
                // Backup & Database Security (Conditional)
                new QuizQuestion
                {
                    Id = 15,
                    Question = "Do you make regular backups of important systems/data?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Backup & Recovery",
                    Explanation = "Regular backups are essential for business continuity and ransomware recovery."
                },
                new QuizQuestion
                {
                    Id = 16,
                    Question = "Have you successfully TESTED a backup restore within the last 3 months?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Backup & Recovery",
                    Explanation = "Testing backups ensures they work when needed and validates recovery procedures."
                },
                new QuizQuestion
                {
                    Id = 17,
                    Question = "Is data at rest encrypted in your databases/storage that hold personal or sensitive data?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Data Protection",
                    Explanation = "Encryption at rest protects sensitive data from unauthorized access even if storage is compromised."
                },
                new QuizQuestion
                {
                    Id = 18,
                    Question = "Do you review user accounts regularly (remove ex-staff, rotate passwords/keys, least privilege)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Access Management",
                    Explanation = "Regular account reviews prevent unauthorized access and enforce least privilege principles."
                },
                
                // Email Security (Conditional)
                new QuizQuestion
                {
                    Id = 19,
                    Question = "Do you have SPF configured for your email domain?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Email Security",
                    Explanation = "SPF prevents email spoofing by specifying which servers can send email for your domain."
                },
                new QuizQuestion
                {
                    Id = 20,
                    Question = "Do you have DKIM configured for your email domain?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Email Security",
                    Explanation = "DKIM provides email authentication through digital signatures to prevent tampering."
                },
                new QuizQuestion
                {
                    Id = 21,
                    Question = "Do you have a DMARC policy set to at least 'quarantine' or 'reject'?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Email Security",
                    Explanation = "DMARC policies enforce SPF and DKIM to prevent email spoofing and phishing attacks."
                },
                
                // Incident Response
                new QuizQuestion
                {
                    Id = 22,
                    Question = "Do you have a written incident response plan (who does what, steps, and notifications)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Incident Response",
                    Explanation = "A written incident response plan ensures organized and effective response to security incidents."
                },
                new QuizQuestion
                {
                    Id = 23,
                    Question = "Have you had any internal or external security check/audit in the last 3 years?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Security Assessment",
                    Explanation = "Regular security audits help identify vulnerabilities and validate security controls."
                },
                
                // Incident History
                new QuizQuestion
                {
                    Id = 24,
                    Question = "Have you had any cybersecurity incidents in the last 12 months?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Incident History",
                    Explanation = "Recent incidents indicate potential security gaps that need to be addressed."
                },
                new QuizQuestion
                {
                    Id = 25,
                    Question = "In the last 6 months, did you have two or more incidents with the same root cause?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Incident Analysis",
                    Explanation = "Repeated incidents with the same root cause indicate systemic security issues."
                },
                new QuizQuestion
                {
                    Id = 26,
                    Question = "Did the direct financial loss from incidents reach or exceed 5% of last year's total revenue?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Impact Assessment",
                    Explanation = "Significant financial impact from incidents may trigger additional reporting requirements."
                },
                
                // Reporting Capabilities
                new QuizQuestion
                {
                    Id = 27,
                    Question = "Are you prepared to submit three-stage reports to the Cybersecurity Agency (Initial, Update, Final)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Compliance Reporting",
                    Explanation = "Multi-stage incident reporting is required for regulatory compliance in many jurisdictions."
                },
                new QuizQuestion
                {
                    Id = 28,
                    Question = "Can you export incident reports as PDF?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Reporting Capabilities",
                    Explanation = "PDF export capabilities facilitate incident documentation and regulatory reporting."
                },
                new QuizQuestion
                {
                    Id = 29,
                    Question = "Can you export incident reports as JSON?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Reporting Capabilities",
                    Explanation = "JSON export enables automated processing and integration with other systems."
                },
                new QuizQuestion
                {
                    Id = 30,
                    Question = "Can you submit incident reports via an API (automatic submission)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Automated Reporting",
                    Explanation = "API submission enables automated and timely incident reporting to authorities."
                },
                new QuizQuestion
                {
                    Id = 31,
                    Question = "Can you submit incident reports by manual upload?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Manual Reporting",
                    Explanation = "Manual upload capabilities provide backup reporting methods when automation fails."
                },
                new QuizQuestion
                {
                    Id = 32,
                    Question = "Do you have a person responsible for incident reporting (owner/manager/IT)?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Governance",
                    Explanation = "Designated incident reporting responsibility ensures accountability and timely reporting."
                },
                
                // Advanced Requirements
                new QuizQuestion
                {
                    Id = 33,
                    Question = "If you are in a critical sector, can you provide automated/direct access for the Agency to incident notifications when required?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Critical Sector Compliance",
                    Explanation = "Critical sectors may require direct agency access for incident monitoring and response."
                },
                new QuizQuestion
                {
                    Id = 34,
                    Question = "If you use cloud services, do you regularly check for public files, overly open permissions, or unused exposed services?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Cloud Security",
                    Explanation = "Regular cloud security audits prevent data exposure and unauthorized access."
                },
                new QuizQuestion
                {
                    Id = 35,
                    Question = "Do you run vulnerability scans or dependency checks for known risks?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Vulnerability Management",
                    Explanation = "Regular vulnerability scanning identifies security weaknesses before they can be exploited."
                },
                new QuizQuestion
                {
                    Id = 36,
                    Question = "Have you defined and tested recovery objectives (RPO/RTO), including a backup restore exercise?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = -1,
                    Category = "Business Continuity",
                    Explanation = "Defined and tested recovery objectives ensure business continuity after incidents."
                }
            };
        }

        public List<QuizQuestion> GetConditionalQuestions()
        {
            // Return all questions - the conditional logic will be handled in the form
            return questions.OrderBy(q => q.Id).ToList();
        }

        public int GetNextQuestionId(int currentQuestionId, Dictionary<int, string> previousAnswers)
        {
            // Implement the specific branching logic from the JSON
            switch (currentQuestionId)
            {
                case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                    // Basic questions flow sequentially
                    return currentQuestionId + 1;
                
                case 8: case 9: case 10:
                    // Security measures flow sequentially
                    return currentQuestionId + 1;
                
                case 11:
                    // After Q11, check if website exists (Q4 = Yes) to show web security questions
                    if (previousAnswers.ContainsKey(4) && previousAnswers[4] == "Yes")
                        return 12; // Go to website security questions
                    else if (previousAnswers.ContainsKey(5) && previousAnswers[5] == "Yes")
                        return 15; // Go to backup questions
                    else if (previousAnswers.ContainsKey(6) && previousAnswers[6] == "Yes")
                        return 19; // Go to email questions
                    else
                        return 22; // Skip to incident response
                
                case 12: case 13:
                    return currentQuestionId + 1; // Web security questions flow
                
                case 14:
                    // After web security, check if database exists (Q5 = Yes)
                    if (previousAnswers.ContainsKey(5) && previousAnswers[5] == "Yes")
                        return 15; // Go to backup questions
                    else
                        return 19; // Skip to email questions
                
                case 15:
                    // Backup question - only if Q5 = Yes
                    return 16;
                
                case 16:
                    // Backup test - only if Q15 = Yes
                    return 17;
                
                case 17:
                    // Encryption - only if Q5 = Yes AND Q3 = Yes
                    return 18;
                
                case 18:
                    // User review - check if email domain exists (Q6 = Yes)
                    if (previousAnswers.ContainsKey(6) && previousAnswers[6] == "Yes")
                        return 19; // Go to email security
                    else
                        return 22; // Skip to incident response
                
                case 19: case 20:
                    return currentQuestionId + 1; // Email security flow
                
                case 21:
                    return 22; // After email security, go to incident response
                
                case 22:
                    return 23; // Go to next incident response question
                
                case 23:
                    return 24; // Go to incident history
                
                case 24:
                    // If no incidents (Q24 = No), skip incident-specific questions
                    if (previousAnswers.ContainsKey(24) && previousAnswers[24] == "No")
                        return 28; // Skip to reporting capabilities
                    else
                        return 25; // Continue with incident questions
                
                case 25: case 26:
                    return currentQuestionId + 1; // Incident analysis flow
                
                case 27:
                    return 28; // Go to reporting capabilities
                
                case 28: case 29: case 30: case 31:
                    return currentQuestionId + 1; // Reporting capabilities flow
                
                case 32:
                    // Check for advanced requirements
                    if (previousAnswers.ContainsKey(2) && previousAnswers[2] == "Yes")
                        return 33; // Critical sector question
                    else if (previousAnswers.ContainsKey(7) && previousAnswers[7] == "Yes")
                        return 34; // Cloud security question
                    else
                        return 35; // Vulnerability scanning
                
                case 33:
                    // After critical sector, check cloud services
                    if (previousAnswers.ContainsKey(7) && previousAnswers[7] == "Yes")
                        return 34;
                    else
                        return 35;
                
                case 34:
                    return 35; // Go to vulnerability scanning
                
                case 35:
                    return 36; // Final question
                
                case 36:
                    return -1; // End of assessment
                
                default:
                    return -1; // End of assessment
            }
        }

        public bool ShouldShowQuestion(int questionId, Dictionary<int, string> previousAnswers)
        {
            // Check if question should be shown based on conditions
            switch (questionId)
            {
                case 12: case 13: case 14:
                    // Website security questions - only if Q4 = Yes
                    return previousAnswers.ContainsKey(4) && previousAnswers[4] == "Yes";
                
                case 15: case 18:
                    // Backup and user review - only if Q5 = Yes
                    return previousAnswers.ContainsKey(5) && previousAnswers[5] == "Yes";
                
                case 16:
                    // Backup test - only if Q15 = Yes
                    return previousAnswers.ContainsKey(15) && previousAnswers[15] == "Yes";
                
                case 17:
                    // Encryption - only if Q5 = Yes AND Q3 = Yes
                    return previousAnswers.ContainsKey(5) && previousAnswers[5] == "Yes" &&
                           previousAnswers.ContainsKey(3) && previousAnswers[3] == "Yes";
                
                case 19: case 20: case 21:
                    // Email security - only if Q6 = Yes
                    return previousAnswers.ContainsKey(6) && previousAnswers[6] == "Yes";
                
                case 25: case 26: case 27:
                    // Incident-specific questions - only if Q24 = Yes
                    return previousAnswers.ContainsKey(24) && previousAnswers[24] == "Yes";
                
                case 33:
                    // Critical sector - only if Q2 = Yes
                    return previousAnswers.ContainsKey(2) && previousAnswers[2] == "Yes";
                
                case 34:
                    // Cloud security - only if Q7 = Yes
                    return previousAnswers.ContainsKey(7) && previousAnswers[7] == "Yes";
                
                case 35:
                    // Vulnerability scanning - if Q2 = Yes OR Q7 = Yes
                    return (previousAnswers.ContainsKey(2) && previousAnswers[2] == "Yes") ||
                           (previousAnswers.ContainsKey(7) && previousAnswers[7] == "Yes");
                
                case 36:
                    // Recovery objectives - if Q2 = Yes OR Q5 = Yes OR Q7 = Yes
                    return (previousAnswers.ContainsKey(2) && previousAnswers[2] == "Yes") ||
                           (previousAnswers.ContainsKey(5) && previousAnswers[5] == "Yes") ||
                           (previousAnswers.ContainsKey(7) && previousAnswers[7] == "Yes");
                
                default:
                    // All other questions are always shown
                    return true;
            }
        }

        public List<QuizQuestion> GetRandomQuestions(int count = 5)
        {
            // For backward compatibility, return first few questions
            return questions.Take(count).ToList();
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

        public AssessmentResult ConvertToAssessmentResult(QuizResult quizResult, Dictionary<int, string> userAnswers)
        {
            var assessmentResult = new AssessmentResult
            {
                Target = "Organization Cybersecurity Assessment",
                ScanTime = quizResult.Session.StartTime,
                Success = quizResult.Session.IsCompleted,
                SecurityScore = quizResult.Session.Score,
                SecurityGrade = quizResult.Grade
            };

            // Group questions by category
            var categorizedQuestions = quizResult.Questions
                .Where(q => userAnswers.ContainsKey(q.Id))
                .GroupBy(q => q.Category)
                .ToList();

            foreach (var categoryGroup in categorizedQuestions)
            {
                var category = new AssessmentCategory
                {
                    Name = categoryGroup.Key,
                    Items = new List<AssessmentItem>()
                };

                int categoryScore = 0;
                int categoryMaxScore = 0;

                foreach (var question in categoryGroup)
                {
                    var answer = quizResult.Session.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                    var userAnswer = userAnswers.ContainsKey(question.Id) ? userAnswers[question.Id] : "No answer";
                    
                    var questionScore = answer != null ? CalculateSecurityScore(question, answer.SelectedAnswerIndex) : 0;
                    categoryScore += questionScore;
                    categoryMaxScore += 4; // Max score per question

                    var item = new AssessmentItem
                    {
                        Question = question.Question,
                        Answer = userAnswer,
                        Status = GetAnswerStatus(questionScore),
                        Score = questionScore,
                        Explanation = question.Explanation,
                        Category = question.Category
                    };

                    category.Items.Add(item);
                }

                category.Score = categoryMaxScore > 0 ? (int)((double)categoryScore / categoryMaxScore * 100) : 0;
                category.Level = GetSecurityLevel(category.Score);
                category.Recommendations = GenerateCategoryRecommendations(categoryGroup.Key, category.Items);

                assessmentResult.Categories.Add(category);
            }

            // Store every single answer with detailed information
            assessmentResult.AllAnswers = PopulateDetailedAnswers(quizResult, userAnswers);
            assessmentResult.RawAnswerData = userAnswers.ToDictionary(
                kvp => $"Q{kvp.Key}", 
                kvp => kvp.Value
            );

            // Generate overall recommendations and summary
            assessmentResult.Recommendations = GenerateOverallRecommendations(assessmentResult.Categories, userAnswers);
            assessmentResult.Warnings = GenerateWarnings(assessmentResult.Categories, userAnswers);
            assessmentResult.Summary = GenerateAssessmentSummary(quizResult, userAnswers, assessmentResult.Categories);

            return assessmentResult;
        }

        private List<DetailedAnswer> PopulateDetailedAnswers(QuizResult quizResult, Dictionary<int, string> userAnswers)
        {
            var detailedAnswers = new List<DetailedAnswer>();

            foreach (var question in quizResult.Questions.Where(q => userAnswers.ContainsKey(q.Id)))
            {
                var answer = quizResult.Session.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var selectedIndex = answer?.SelectedAnswerIndex ?? -1;
                var securityScore = answer != null ? CalculateSecurityScore(question, selectedIndex) : 0;
                
                var detailedAnswer = new DetailedAnswer
                {
                    QuestionId = question.Id,
                    Question = question.Question,
                    Category = question.Category,
                    SelectedAnswer = userAnswers[question.Id],
                    SelectedIndex = selectedIndex,
                    AllOptions = new List<string>(question.Options),
                    SecurityScore = securityScore,
                    SecurityLevel = GetSecurityLevel(securityScore * 25), // Convert 1-4 to percentage
                    Explanation = question.Explanation,
                    AnsweredAt = answer?.AnsweredAt ?? DateTime.Now,
                    Impact = GetAnswerImpact(question, selectedIndex),
                    Implications = GetAnswerImplications(question, selectedIndex)
                };

                detailedAnswers.Add(detailedAnswer);
            }

            return detailedAnswers.OrderBy(a => a.QuestionId).ToList();
        }

        private string GetAnswerImpact(QuizQuestion question, int selectedIndex)
        {
            // Determine the impact based on the question category and selected answer
            var impact = selectedIndex switch
            {
                0 => "Positive - Strong security posture",
                1 => "Neutral - Adequate security measures",
                2 => "Concerning - Basic security gaps",
                3 => "Critical - Significant security risk",
                _ => "Unknown - No response provided"
            };

            // Add category-specific context
            var categoryContext = question.Category.ToLower() switch
            {
                var c when c.Contains("security") => " for organizational security framework",
                var c when c.Contains("network") => " for network infrastructure protection",
                var c when c.Contains("access") => " for access control and authentication",
                var c when c.Contains("incident") => " for incident response capabilities",
                var c when c.Contains("compliance") => " for regulatory compliance posture",
                _ => " for overall cybersecurity posture"
            };

            return impact + categoryContext;
        }

        private List<string> GetAnswerImplications(QuizQuestion question, int selectedIndex)
        {
            var implications = new List<string>();

            // Generate implications based on the security score and question context
            var securityScore = CalculateSecurityScore(question, selectedIndex);
            
            switch (securityScore)
            {
                case 4: // Excellent
                    implications.Add("Demonstrates strong security awareness and implementation");
                    implications.Add("Contributes positively to overall security posture");
                    implications.Add("Aligns with cybersecurity best practices");
                    break;
                    
                case 3: // Good
                    implications.Add("Shows adequate security measures in place");
                    implications.Add("Minor improvements could enhance security further");
                    break;
                    
                case 2: // Fair
                    implications.Add("Indicates potential security gaps that need attention");
                    implications.Add("May increase organizational risk if not addressed");
                    implications.Add("Requires security improvements to meet best practices");
                    break;
                    
                case 1: // Poor
                    implications.Add("Represents significant security vulnerability");
                    implications.Add("Immediate attention required to reduce risk");
                    implications.Add("Could lead to security incidents if not remediated");
                    implications.Add("May impact compliance with security standards");
                    break;
                    
                default:
                    implications.Add("Unable to assess security impact without response");
                    break;
            }

            // Add question-specific implications
            if (question.Question.ToLower().Contains("multi-factor") || question.Question.ToLower().Contains("mfa"))
            {
                if (securityScore <= 2)
                    implications.Add("Lack of MFA significantly increases account compromise risk");
            }
            
            if (question.Question.ToLower().Contains("backup"))
            {
                if (securityScore <= 2)
                    implications.Add("Inadequate backup strategy could result in data loss during incidents");
            }

            if (question.Question.ToLower().Contains("training") || question.Question.ToLower().Contains("awareness"))
            {
                if (securityScore <= 2)
                    implications.Add("Limited security training increases human error and phishing susceptibility");
            }

            return implications;
        }

        private string GetAnswerStatus(int score)
        {
            return score switch
            {
                4 => "Excellent",
                3 => "Good",
                2 => "Fair", 
                1 => "Poor",
                _ => "Critical"
            };
        }

        private string GetSecurityLevel(int percentage)
        {
            return percentage switch
            {
                >= 80 => "High",
                >= 60 => "Medium",
                >= 40 => "Low",
                _ => "Critical"
            };
        }

        private List<string> GenerateCategoryRecommendations(string category, List<AssessmentItem> items)
        {
            var recommendations = new List<string>();
            var poorItems = items.Where(i => i.Score <= 2).ToList();

            foreach (var item in poorItems)
            {
                switch (category)
                {
                    case "Business Profile":
                        if (item.Question.Contains("critical sector"))
                            recommendations.Add("Consider implementing enhanced security measures required for critical sectors.");
                        break;
                    case "Access Control":
                        if (item.Question.Contains("Two-Factor"))
                            recommendations.Add("Implement Multi-Factor Authentication (MFA) for all administrator and privileged accounts immediately.");
                        break;
                    case "Web Security":
                        if (item.Question.Contains("HTTPS"))
                            recommendations.Add("Ensure all web applications enforce HTTPS with valid TLS certificates.");
                        if (item.Question.Contains("security headers"))
                            recommendations.Add("Implement security headers (HSTS, CSP, X-Frame-Options) on all web applications.");
                        break;
                    case "Email Security":
                        if (item.Question.Contains("SPF"))
                            recommendations.Add("Configure SPF records for your email domain to prevent spoofing.");
                        if (item.Question.Contains("DKIM"))
                            recommendations.Add("Implement DKIM signing for email authentication.");
                        if (item.Question.Contains("DMARC"))
                            recommendations.Add("Set up DMARC policy with 'quarantine' or 'reject' action.");
                        break;
                    case "Backup & Recovery":
                        if (item.Question.Contains("backup"))
                            recommendations.Add("Implement regular automated backups with offsite storage.");
                        if (item.Question.Contains("tested"))
                            recommendations.Add("Regularly test backup restoration procedures (at least quarterly).");
                        break;
                }
            }

            return recommendations;
        }

        private List<string> GenerateOverallRecommendations(List<AssessmentCategory> categories, Dictionary<int, string> userAnswers)
        {
            var recommendations = new List<string>();
            var criticalCategories = categories.Where(c => c.Score < 40).ToList();
            var lowCategories = categories.Where(c => c.Score >= 40 && c.Score < 60).ToList();

            if (criticalCategories.Any())
            {
                recommendations.Add("CRITICAL: Immediate action required in the following areas: " + 
                                  string.Join(", ", criticalCategories.Select(c => c.Name)));
            }

            if (lowCategories.Any())
            {
                recommendations.Add("PRIORITY: Improve security measures in: " + 
                                  string.Join(", ", lowCategories.Select(c => c.Name)));
            }

            // Add specific recommendations based on answers
            if (userAnswers.ContainsKey(8) && userAnswers[8] == "No")
                recommendations.Add("Enable Two-Factor Authentication (2FA/MFA) for all administrative accounts immediately.");

            if (userAnswers.ContainsKey(10) && userAnswers[10] == "No")
                recommendations.Add("Establish a regular patch management process for all systems and applications.");

            if (userAnswers.ContainsKey(22) && userAnswers[22] == "No")
                recommendations.Add("Develop and document a comprehensive incident response plan.");

            return recommendations;
        }

        private List<string> GenerateWarnings(List<AssessmentCategory> categories, Dictionary<int, string> userAnswers)
        {
            var warnings = new List<string>();

            // Check for high-risk scenarios
            if (userAnswers.ContainsKey(24) && userAnswers[24] == "Yes")
                warnings.Add("Recent cybersecurity incidents detected - review and strengthen security measures.");

            if (userAnswers.ContainsKey(2) && userAnswers[2] == "Yes")
                warnings.Add("Critical sector organization - enhanced compliance requirements may apply.");

            if (userAnswers.ContainsKey(3) && userAnswers[3] == "Yes" && 
                userAnswers.ContainsKey(17) && userAnswers[17] == "No")
                warnings.Add("Personal data collection without encryption at rest poses GDPR compliance risk.");

            return warnings;
        }

        private AssessmentSummary GenerateAssessmentSummary(QuizResult quizResult, Dictionary<int, string> userAnswers, List<AssessmentCategory> categories)
        {
            var summary = new AssessmentSummary
            {
                TotalQuestions = quizResult.Session.TotalQuestions,
                QuestionsAnswered = userAnswers.Count,
                CompletionPercentage = quizResult.Session.TotalQuestions > 0 ? 
                    (double)userAnswers.Count / quizResult.Session.TotalQuestions * 100 : 0
            };

            // Determine organization type
            if (userAnswers.ContainsKey(1) && userAnswers[1] == "Yes")
                summary.OrganizationType = "Small/Medium Enterprise (SME)";
            else
                summary.OrganizationType = "Large Enterprise";

            // Identify applicable sectors
            if (userAnswers.ContainsKey(2) && userAnswers[2] == "Yes")
                summary.ApplicableSectors.Add("Critical Infrastructure Sector");
            if (userAnswers.ContainsKey(7) && userAnswers[7] == "Yes")
                summary.ApplicableSectors.Add("Cloud Services User");
            if (userAnswers.ContainsKey(3) && userAnswers[3] == "Yes")
                summary.ApplicableSectors.Add("Personal Data Processor");

            // Generate key findings
            var excellentCategories = categories.Where(c => c.Score >= 80).Select(c => c.Name).ToList();
            var criticalCategories = categories.Where(c => c.Score < 40).Select(c => c.Name).ToList();

            if (excellentCategories.Any())
                summary.KeyFindings.Add("Strong security posture in: " + string.Join(", ", excellentCategories));
            if (criticalCategories.Any())
                summary.CriticalIssues.AddRange(criticalCategories.Select(c => $"Critical gaps in {c}"));

            return summary;
        }
    }
}
