using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizCode.Domain
{
    public class Answer
    {
        public Answer(Question question, string selectedChoiceCode, DateTime answeredAtUtc)
        {
            Question = question;
            SelectedChoiceCode = selectedChoiceCode?.Trim()!;
            AnsweredAtUtc = answeredAtUtc;

            EnsureValid();
        }

        public Question Question { get; }
        public string SelectedChoiceCode { get; }
        public DateTime AnsweredAtUtc { get; }

        public bool IsCorrect => Question.IsCorrect(SelectedChoiceCode);

        private void EnsureValid()
        {
            if (Question == null)
                throw new DomainException("Question must not be null.");
            if (string.IsNullOrEmpty(SelectedChoiceCode))
                throw new DomainException("SelectedChoiceCode must not be null or empty.");
            if (!Question.HasChoice(SelectedChoiceCode))
                throw new DomainException($"SelectedChoiceCode '{SelectedChoiceCode}' hör inte till frågan.");
            if (AnsweredAtUtc == default)
                throw new DomainException("AnsweredAtUtc must be a valid date time.");
        }
    }
}
