using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trier
{
    public class Answer<T> : AnswerBase
    {
        public T Value { get; protected set; }

        private Answer(string message) : base(message) { }

        public static Answer<T> Prepare(string action) => new(action);

        public Answer<T> Ok(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Input parameter can not be null - please check it and return Error when null");
            }
            IsSuccess = true;
            Value = value;
            return this;
        }

        public Answer<T> Error()
        {
            return this;
        }

        public override IAnswerBase Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
