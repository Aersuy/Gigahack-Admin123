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
        private int currentQuestionIndex = 0;
        private List<QuizQuestion> questions;

        public QuizForm()
        {
            InitializeComponent();
            quiz = new Quiz();
            StartNewAssessment();
        }

        private void StartNewAssessment()
        {
            // Get 8 random questions for assessment
            questions = quiz.GetRandomQuestions(8);
            currentQuizResult = quiz.CreateQuizSession(questions);
            currentQuestionIndex = 0;

            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var question = questions[currentQuestionIndex];

                lblQuestionNumber.Text = $"Question {currentQuestionIndex + 1} of {questions.Count}";
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

                btnNext.Text = currentQuestionIndex == questions.Count - 1 ? "Complete Assessment" : "Next Question";
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
                quiz.SubmitAnswer(currentQuizResult, questions[currentQuestionIndex].Id, selectedAnswerIndex);
            }

            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                DisplayCurrentQuestion();
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
            var resultsForm = new QuizResultsForm(currentQuizResult, questions);
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
