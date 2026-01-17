using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizCode.Domain
{
    public class Choice
    {
        public Choice(string code, string option)
        {
            Id = Guid.NewGuid();
            Code = code;
            Text = option;

            EnsureValid();
        }

        public Guid Id { get; }
        public string Code { get; }
        public string Text { get; }

        private void EnsureValid()
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                throw new DomainException("Code must not be null or empty");
            }

            if (string.IsNullOrEmpty(Text))
            {
                throw new DomainException("Text must not be null or empty");
            }
        }
    }
}
