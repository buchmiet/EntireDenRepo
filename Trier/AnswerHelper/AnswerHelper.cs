using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trier.DialogService;

namespace Trier
{
    public static class AnswerHelper
    {
        internal static Dictionary<object, Dictionary<Exception, IAsyncDialogService>> SubscribedExceptions = new();

        public static Answer Subscribe(object sender, Dictionary<Exception,IAsyncDialogService> e)
        {
            var answer = Answer.Prepare(DevComms.SubscribingServiceToTheListOfExceptionsThatMayBeCaughtWhileExecutingActions);
            if (SubscribedExceptions.ContainsKey(sender))
            {
                return answer.Error(string.Format(
                    DevComms
                        .HasAlreadySubscribedToAddListenersToExceptionsThrownToKeepTheCodeCleanAndSafeThereIsNoMethodToAddMoreExceptionsListenersToOneSubscriberIfThisIsDifficultToExecuteInYourArchitectureHelpDevelopingAndAddThisMethod,
                    sender.GetType().Name));
            }
            try
            {
                SubscribedExceptions.Add(sender, e);
            }
            catch (Exception ex)
            {
                return answer.Error(string.Format(
                    DevComms
                        .WasThrownWithTheFollowingError,
                    ex.GetType().Name,ex.Message));
            }
            
            return answer;
        }

        public static async Task<TAnswer> TryAsync<TAnswer>(Func<Task<TAnswer>> action) where TAnswer : IAnswerBase
        {
            var response = await action();
            return response;
        }

        public static Task<TAnswer> TryAsync<TAnswer>(Task<TAnswer> action) where TAnswer : IAnswerBase => action;
        

        //public static async Task<(bool, TResponse)> TryAsync<TResponse>(Func<Task<TResponse>> action) where TResponse : IAnswerBase
        //{
        //    TResponse response = await action();
        //    return (response.IsSuccess, response);
        //}

        //public static bool Try<TAnswer>(Func<TAnswer> action, out TAnswer response) where TAnswer : IAnswerBase
        //{
        //    response = action();
        //    return response.IsSuccess;
        //}

        public static bool Try<TAnswer>(Func<TAnswer> action, out TAnswer answer) where TAnswer : IAnswerBase
        {
            answer = action();
            if (answer.IsSuccess)
            {
                return true;
            }

            if (answer.HasException)
            {
                if (SubscribedExceptions.TryGetValue(answer.Sender, out var exceptions))
                {
               //     if 
                }
            }

            return true;
        }
    }
}
