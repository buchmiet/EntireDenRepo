using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trier
{
    public class Answer : AnswerBase
    {
        private Answer(string message) : base(message) { }
        public static Answer Prepare(string action) => new(action);

        public Answer Error(string errorMessage)
        {
            ErrorDescription= errorMessage;
            return this;
        }

        public Answer Error(IAnswerBase answer)
        {
            
            return this;
        }

        public Answer Error()
        {
            return this;
        }

        public override IAnswerBase Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
