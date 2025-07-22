using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trier
{
    public interface IAnswerService
    {
        bool Try<TAnswer>(Func<TAnswer> action, out TAnswer answer) where TAnswer : IAnswerBase;
        void PrepareTry(object target);
    }

    public class AnswerService : IAnswerService
    {
        public bool Try<TAnswer>(Func<TAnswer> action, out TAnswer answer) where TAnswer : IAnswerBase
        {
            try
            {
                answer = action();
                return true;
            }
            catch
            {
                answer = default;
                return false;
            }
        }

        public void PrepareTry(object target)
        {
            // Implementacja PrepareTry
        }
    }
}
