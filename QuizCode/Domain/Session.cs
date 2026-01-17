using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizCode.Domain
{
    public class Session
    {
        public Guid Id { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? FinishedAtUtc { get; set; }

        public int QuestionCount { get; set; }
        public IList<Answer> Answers { get; } = new List<Answer>();

        public int Score => Answers.Count(a => a.IsCorrect);

        public static Session Create(int count)
        {
            return new Session
            {
                Id = Guid.NewGuid(),
                StartedAtUtc = DateTime.UtcNow,
                QuestionCount = count
            };
        }

        public void SubmitAnswer(Question question, string selectedChoiceCode, DateTime answeredAtUtc)
        {
            EnsureSessionActive();

            if (Answers.Any(a => a.Question.Code.Equals(question.Code, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException($"Frågan '{question.Code}' har redan besvarats i denna session.");

            var answer = new Answer(question, selectedChoiceCode, answeredAtUtc);
            Answers.Add(answer);

            EnsureValid();
        }

        private void EnsureSessionActive()
        {
            EnsureStarted();
            if (FinishedAtUtc != null)
                throw new DomainException("Session är avslutad. Det går inte att registrera fler svar");
        }

        private void EnsureStarted()
        {
            if (Id == Guid.Empty)
                throw new DomainException("Id måste sättas innan session används.");

            if (StartedAtUtc == default)
                throw new DomainException("StartedAtUtc vara sätt till en giltig UTC-Datum");

            if (QuestionCount <= 0)
                throw new DomainException("QuestionCount måste vara > 0 eller högre än 0");
        }

        private void EnsureValid()
        {
            if (Id == Guid.Empty)
                throw new DomainException("Id får inte vara tomt.");
            if (QuestionCount <= 0)
                throw new DomainException("QuestionCount måste vara större än 0.");
            if (StartedAtUtc == default)
                throw new DomainException("StartedAtUtc måste vara satt");
            if (FinishedAtUtc is { } f)
            {
                if (f < StartedAtUtc)
                    throw new DomainException("FinishedAtUtc får inte vara mindre än StartedAtUtc.");
            }

            var duplicateCodes = Answers
                .GroupBy(a => a.Question.Code, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCodes.Any())
                throw new DomainException($"Dubbla svar på samma fråga: {string.Join(", ", duplicateCodes)}");

            foreach (var a in Answers)
            {
                if (a.AnsweredAtUtc < StartedAtUtc)
                {
                    throw new DomainException($"Svarstid ({a.AnsweredAtUtc}) kan inte vara före sessionens start ({StartedAtUtc}).");
                }

                if (FinishedAtUtc is DateTime finished && a.AnsweredAtUtc > finished)
                    throw new DomainException($"Svarstid ({a.AnsweredAtUtc}) kan inte vara efter sessions slut ({finished}).");
                //Kolla vid senare tidfälle om det går att använda "if (a.AnsweredAtUtc > FinishedAtUtc.Value)"
            }

            if (Answers.Count > QuestionCount)
                throw new DomainException($"Antal svar ({Answers.Count}) kan inte överstiga mängden frågor ({QuestionCount})");
        }
    }
}
