using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM.DataClasses
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public int? NextQuestionId { get; set; } = null;
        public List<QuizCondition> Conditions { get; set; } = new List<QuizCondition>();
    }

    public class QuizCondition
    {
        public int RequiredQuestionId { get; set; }
        public string RequiredAnswer { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true; // true for "all", false for "any"
    }

    public class QuizAnswer
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; } = DateTime.Now;
    }

    public class QuizSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsCompleted { get; set; }
        
        public double PercentageScore => TotalQuestions > 0 ? (double)Score / (TotalQuestions * 4) * 100 : 0; // Max score is 4 per question
    }

    public class QuizResult
    {
        public QuizSession Session { get; set; } = new QuizSession();
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public string Grade => Session.PercentageScore switch
        {
            >= 90 => "A",
            >= 80 => "B", 
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }

    // Assessment result structure compatible with scan results for LLM integration
    public class AssessmentResult
    {
        public string Target { get; set; } = "Organization Assessment";
        public DateTime ScanTime { get; set; } = DateTime.Now;
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public int SecurityScore { get; set; }
        public string SecurityGrade { get; set; } = string.Empty;
        public List<AssessmentCategory> Categories { get; set; } = new List<AssessmentCategory>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
        public AssessmentSummary Summary { get; set; } = new AssessmentSummary();
    }

    public class AssessmentCategory
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Level { get; set; } = string.Empty;
        public List<AssessmentItem> Items { get; set; } = new List<AssessmentItem>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    public class AssessmentItem
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class AssessmentSummary
    {
        public int TotalQuestions { get; set; }
        public int QuestionsAnswered { get; set; }
        public double CompletionPercentage { get; set; }
        public string OrganizationType { get; set; } = string.Empty;
        public List<string> ApplicableSectors { get; set; } = new List<string>();
        public List<string> KeyFindings { get; set; } = new List<string>();
        public List<string> CriticalIssues { get; set; } = new List<string>();
        public List<string> ComplianceGaps { get; set; } = new List<string>();
    }
}
