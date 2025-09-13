using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LLM.Logic;
using LLM.DataClasses;

namespace Gigahack_Admin123
{
    public partial class QuizForm : Form
    {
        private Quiz quiz;
        private QuizResult currentQuizResult;
        private int currentQuestionId = 1; // Track by ID instead of index
        private List<QuizQuestion> allQuestions;
        private Dictionary<int, string> userAnswers = new Dictionary<int, string>();

        public QuizForm()
        {
            InitializeComponent();
            quiz = new Quiz();
            StartNewAssessment();
        }

        private void StartNewAssessment()
        {
            // Get all questions for conditional assessment
            allQuestions = quiz.GetConditionalQuestions();
            currentQuizResult = quiz.CreateQuizSession(allQuestions);
            currentQuestionId = 1;
            userAnswers.Clear();
            
            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            var question = allQuestions.FirstOrDefault(q => q.Id == currentQuestionId);
            if (question != null)
            {
                // Calculate question number for display
                var questionNumber = userAnswers.Count + 1;
                
                lblQuestionNumber.Text = $"Question {questionNumber}";
                lblQuestion.Text = question.Question;
                lblCategory.Text = $"Category: {question.Category}";
                
                // Clear previous options
                foreach (Control control in panelOptions.Controls)
                {
                    if (control is RadioButton)
                    {
                        control.Dispose();
                    }
                }
                panelOptions.Controls.Clear();
                
                // Add new options
                for (int i = 0; i < question.Options.Count; i++)
                {
                    var radioButton = new RadioButton
                    {
                        Text = question.Options[i],
                        Location = new Point(15, 15 + (i * 60)),
                        Size = new Size(500, 50),
                        Tag = i,
                        Font = new Font("Segoe UI", 11),
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    panelOptions.Controls.Add(radioButton);
                }
                
                // Determine if this is the last question
                var nextQuestionId = quiz.GetNextQuestionId(currentQuestionId, userAnswers);
                btnNext.Text = nextQuestionId == -1 ? "Complete Assessment" : "Next Question";
                btnNext.Enabled = false; // Enable when an option is selected
                
                // Add event handlers
                foreach (Control control in panelOptions.Controls)
                {
                    if (control is RadioButton rb)
                    {
                        rb.CheckedChanged += RadioButton_CheckedChanged;
                    }
                }
            }
            else
            {
                // No more questions, finish assessment
                FinishQuiz();
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            btnNext.Enabled = panelOptions.Controls.OfType<RadioButton>().Any(rb => rb.Checked);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Get selected answer
            var selectedRadioButton = panelOptions.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked);
            if (selectedRadioButton != null)
            {
                int selectedAnswerIndex = (int)selectedRadioButton.Tag;
                var currentQuestion = allQuestions.FirstOrDefault(q => q.Id == currentQuestionId);
                
                if (currentQuestion != null)
                {
                    // Store the answer text for conditional logic
                    string selectedAnswerText = currentQuestion.Options[selectedAnswerIndex];
                    userAnswers[currentQuestionId] = selectedAnswerText;
                    
                    // Submit answer to quiz result
                    quiz.SubmitAnswer(currentQuizResult, currentQuestionId, selectedAnswerIndex);
                }
            }

            // Get next question ID based on conditional logic
            var nextQuestionId = quiz.GetNextQuestionId(currentQuestionId, userAnswers);
            
            if (nextQuestionId > 0)
            {
                // Skip questions that shouldn't be shown based on conditions
                while (nextQuestionId > 0 && !quiz.ShouldShowQuestion(nextQuestionId, userAnswers))
                {
                    nextQuestionId = quiz.GetNextQuestionId(nextQuestionId, userAnswers);
                }
                
                if (nextQuestionId > 0)
                {
                    currentQuestionId = nextQuestionId;
                    DisplayCurrentQuestion();
                }
                else
                {
                    FinishQuiz();
                }
            }
            else
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            quiz.CompleteQuiz(currentQuizResult);
            ShowResults();
        }

        private void ShowResults()
        {
            // Create a list of questions that were actually answered
            var answeredQuestions = allQuestions.Where(q => userAnswers.ContainsKey(q.Id)).ToList();
            var resultsForm = new QuizResultsForm(currentQuizResult, answeredQuestions);
            resultsForm.ShowDialog();
            this.Close();
        }

        private void btnStartOver_Click(object sender, EventArgs e)
        {
            StartNewAssessment();
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            var historyForm = new QuizHistoryForm(quiz.GetQuizHistory());
            historyForm.ShowDialog();
        }

        private void lblCategory_Click(object sender, EventArgs e)
        {

        }
    }
}
