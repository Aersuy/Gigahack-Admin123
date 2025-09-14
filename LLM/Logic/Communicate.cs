using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LLM.DataClasses;

namespace LLM.Logic
{
    public class Communicate
    {
        const string reportPromptBegin = @"You are a cybersecurity analyst with 20 years of experience preparing a two-page security report for company leadership from the information provided. the first page must consist of a technical report using the data from the system scan. second page must consist of the information from the IT assesment. YOU DO NOT HALLUCINATE;

DEFINITIONS:
- ""Executive Summary""  - a simple but realistic overview of the vulnerabilities found in the company's cyber-infrastructure, aimed at high ranking company executives 3 short sentences;
- ""Security Overview"" - key findings summed in 3 short sentences;
- ""Key Findings"" - consist of the NetworkSecurity, SystemSecurity, PasswordSecurity and WebSecurity assessment, say where improvements are needed in 1 - 4 short sentences;
- ""Recommendations"" - the actions needed in order to fix the vulnerabilities in key findings in 1 - 4 short sentences;
- ""Conclusion"" - concise and relevant advice for fixing the problems found in 1 sentence;

 1'ST PAGE REQUIREMENTS:
- all text must fit on 1 page;
- Keep it short and easy to read;
- Use plain, simple language for non-technical readers, explain technical words in a simple way;
- Add 1 blank line between every finding in Key Findings and every action in Recommendations;
- add 3 lines between the sections Executive Summary, Security Overview, Key Findings, Recommendations, Conclusion;
- DO NOT HALLUCINATE;
- For the first page use the following section order;

1. Executive Summary  
2. Security Overview 
3. Key Findings  
4. Recommendations  
5. Conclusion  

Below you have the scan data:
";
        const string reportPromptEnd = @" 2'ND PAGE REQUIREMENTS:
- all text must fit on 1 page;
- Keep it short and easy to read;
- Use plain, simple language for non-technical readers, explain technical words in a simple way;
- Add 1 blank line between every finding in Key Findings and every action in Recommendations;
- add 3 lines between the sections Executive Summary, Security Overview, Key Findings, Recommendations, Conclusion;

1. Executive Summary  
2. Security Overview 
3. Key Findings  
4. Recommendations  
5. Conclusion

Below You have the assesment data:";
        public async Task<string> SendMessage(string prompt)
        {
            var http = new HttpClient();
            var url = "http://100.98.187.96:11434/api/generate"; // Home PC Tailscale IP

            var payload = new
            {
                model = "gpt-oss:20b",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var resp = await http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var responseBody = await resp.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            string text = doc.RootElement.GetProperty("response").GetString();

            return text;
        }

        public async Task<string> GenerateReport(OverallReportData data)
        {
            var prompt = $"{reportPromptBegin}\n\n" +
                        $"Target: {data.Target}\n" +
                        $"Overall Score: {data.OverallScore}/100\n" +
                        $"Compliance Level: {data.ComplianceLevel}\n" +
                        $"Scan Date: {data.ScanTime:yyyy-MM-dd HH:mm:ss}\n\n" +
                        
                        // Enhanced security category details with specific suggestions
                        $"DETAILED SECURITY ANALYSIS:\n\n" +
                        FormatCategorySuggestions("Password Security", data.PasswordSecuritySuggestions) +
                        FormatCategorySuggestions("Web Security", data.WebSecuritySuggestions) +
                        FormatCategorySuggestions("Network Security", data.NetworkSecuritySuggestions) +
                        FormatCategorySuggestions("System Security", data.SystemSecuritySuggestions) +
                        FormatCategorySuggestions("Vulnerability Management", data.VulnerabilityManagementSuggestions) +
                        
                        $"TECHNICAL SCAN FINDINGS:\n" +
                        $"Critical Technical Issues:\n{string.Join("\n", data.KeyFindings.Take(5))}\n\n" +

                        $"{reportPromptEnd}\n\n" +
                        // Assessment data integration
                        (data.HasAssessmentData  ?
                           $"These are answers to the questions\n" +
                            $"DEBUG: Assessment data detected - formatting...\n{FormatAssessmentData(data.Assessment)}" : 
                            "DEBUG: No IT Infrastructure Assessment data found.\n\n") +
                        
                        $"INTEGRATED ANALYSIS INSTRUCTIONS:\n" +
                        $"Generate a comprehensive security report that combines both technical scan results and organizational assessment findings. " +
                        $"{(data.HasAssessmentData ? "Correlate technical vulnerabilities with organizational weaknesses identified in the assessment. " : "")}" +
                        $"Prioritize recommendations based on both technical severity and organizational impact. " +
                        $"Focus on actionable steps that address both technical and procedural security gaps. " +
                        $"Present findings in business terms that non-technical leadership can understand and act upon.";
           
            return await SendMessage(prompt);
        }

        private string FormatCategorySuggestions(string categoryName, CategorySuggestions suggestions)
        {
            if (suggestions == null || string.IsNullOrEmpty(suggestions.CategoryName))
                return $"{categoryName}: No data available\n\n";

            var formatted = $"{categoryName} ({suggestions.Score}/100 - {suggestions.ComplianceLevel}):\n" +
                           $"Status: {suggestions.SummaryMessage}\n";

            if (suggestions.CriticalActions.Any())
            {
                formatted += $"URGENT Actions Required:\n";
                foreach (var action in suggestions.CriticalActions.Take(3))
                {
                    formatted += $"- {action}\n";
                }
            }

            if (suggestions.ImmediateSuggestions.Any())
            {
                formatted += $"Immediate Improvements:\n";
                foreach (var suggestion in suggestions.ImmediateSuggestions.Take(3))
                {
                    formatted += $"- {suggestion}\n";
                }
            }

            if (suggestions.BestPractices.Any())
            {
                formatted += $"Best Practices:\n";
                foreach (var practice in suggestions.BestPractices.Take(2))
                {
                    formatted += $"- {practice}\n";
                }
            }

            return formatted + "\n";
        }

        private string FormatAssessmentData(AssessmentResult assessment)
        {
            if (assessment == null) return "";

            var formatted = $"IT INFRASTRUCTURE ASSESSMENT ANALYSIS:\n" +
                           $"Organization Security Grade: {assessment.SecurityGrade}\n" +
                           $"Assessment Score: {assessment.SecurityScore}\n" +
                           $"Completion Rate: {assessment.Summary.QuestionsAnswered}/{assessment.Summary.TotalQuestions} questions answered ({assessment.Summary.CompletionPercentage:F1}%)\n" +
                           $"Organization Type: {assessment.Summary.OrganizationType}\n\n";

            // Add category breakdown
            if (assessment.Categories.Any())
            {
                formatted += $"ASSESSMENT CATEGORY BREAKDOWN:\n";
                foreach (var category in assessment.Categories.OrderByDescending(c => c.Score))
                {
                    formatted += $"• {category.Name}: {category.Score}% ({category.Level})\n";
                    
                    // Include top recommendations for each category
                    if (category.Recommendations.Any())
                    {
                        var topRec = category.Recommendations.FirstOrDefault();
                        if (!string.IsNullOrEmpty(topRec))
                        {
                            formatted += $"  Priority: {topRec}\n";
                        }
                    }
                }
                formatted += "\n";
            }

            // Critical issues (high priority)
            if (assessment.Summary.CriticalIssues.Any())
            {
                formatted += $"CRITICAL ORGANIZATIONAL ISSUES:\n";
                foreach (var issue in assessment.Summary.CriticalIssues.Take(5))
                {
                    formatted += $"• {issue}\n";
                }
                formatted += "\n";
            }

            // Key findings from assessment
            if (assessment.Summary.KeyFindings.Any())
            {
                formatted += $"KEY ORGANIZATIONAL FINDINGS:\n";
                foreach (var finding in assessment.Summary.KeyFindings.Take(5))
                {
                    formatted += $"• {finding}\n";
                }
                formatted += "\n";
            }

            // Compliance gaps
            if (assessment.Summary.ComplianceGaps.Any())
            {
                formatted += $"COMPLIANCE GAPS IDENTIFIED:\n";
                foreach (var gap in assessment.Summary.ComplianceGaps.Take(4))
                {
                    formatted += $"• {gap}\n";
                }
                formatted += "\n";
            }

            // Overall recommendations from assessment
            if (assessment.Recommendations.Any())
            {
                formatted += $"ASSESSMENT-BASED RECOMMENDATIONS:\n";
                foreach (var rec in assessment.Recommendations.Take(5))
                {
                    formatted += $"• {rec}\n";
                }
                formatted += "\n";
            }

            // Applicable sectors for context
            if (assessment.Summary.ApplicableSectors.Any())
            {
                formatted += $"Relevant Industry Sectors: {string.Join(", ", assessment.Summary.ApplicableSectors.Take(3))}\n\n";
            }

            // Include detailed individual answer analysis
            if (assessment.AllAnswers.Any())
            {
                formatted += $"DETAILED INDIVIDUAL RESPONSES ANALYSIS:\n";
                formatted += $"Total Questions Answered: {assessment.AllAnswers.Count}\n\n";

                // Group by security level for prioritized analysis
                var criticalAnswers = assessment.AllAnswers.Where(a => a.SecurityScore <= 1).ToList();
                var concerningAnswers = assessment.AllAnswers.Where(a => a.SecurityScore == 2).ToList();
                var strongAnswers = assessment.AllAnswers.Where(a => a.SecurityScore >= 3).ToList();

                // Critical security gaps (highest priority)
                if (criticalAnswers.Any())
                {
                    formatted += $"CRITICAL SECURITY GAPS ({criticalAnswers.Count} items):\n";
                    foreach (var answer in criticalAnswers.Take(5))
                    {
                        formatted += $"• Q{answer.QuestionId} ({answer.Category}): {answer.Question}\n";
                        formatted += $"  Response: {answer.SelectedAnswer}\n";
                        formatted += $"  Impact: {answer.Impact}\n";
                        if (answer.Implications.Any())
                        {
                            formatted += $"  Key Risk: {answer.Implications.FirstOrDefault()}\n";
                        }
                        formatted += "\n";
                    }
                }

                // Areas needing improvement
                if (concerningAnswers.Any())
                {
                    formatted += $"AREAS REQUIRING IMPROVEMENT ({concerningAnswers.Count} items):\n";
                    foreach (var answer in concerningAnswers.Take(4))
                    {
                        formatted += $"• Q{answer.QuestionId} ({answer.Category}): {answer.SelectedAnswer}\n";
                        formatted += $"  Recommendation: {answer.Explanation}\n";
                    }
                    formatted += "\n";
                }

                // Strong security practices (positive reinforcement)
                if (strongAnswers.Any())
                {
                    formatted += $"STRONG SECURITY PRACTICES ({strongAnswers.Count} items):\n";
                    foreach (var answer in strongAnswers.Take(3))
                    {
                        formatted += $"• {answer.Category}: {answer.SelectedAnswer}\n";
                    }
                    formatted += "\n";
                }

                // Raw answer data for comprehensive analysis
                if (assessment.RawAnswerData.Any())
                {
                    formatted += $"COMPLETE RESPONSE DATASET:\n";
                    foreach (var rawAnswer in assessment.RawAnswerData.Take(10))
                    {
                        formatted += $"{rawAnswer.Key}: {rawAnswer.Value}\n";
                    }
                    if (assessment.RawAnswerData.Count > 10)
                    {
                        formatted += $"... and {assessment.RawAnswerData.Count - 10} more responses\n";
                    }
                    formatted += "\n";
                }
            }

            return formatted;
        }
    }

}