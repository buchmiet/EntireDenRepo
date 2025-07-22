//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Trier
//{
//    [GenerateTryMethod]
//    public partial class ExampleClass
//    {
//        private readonly IAnswerService _answerService;

//        public ExampleClass(IAnswerService answerService)
//        {
//            _answerService = answerService;
//            (_answerService as AnswerService)?.PrepareTry(this);
//        }

//        //public void SomeMethod()
//        //{
//        //    if (Try(() => new SomeAnswer(), out var answer))
//        //    {
//        //        // użyj answer
//        //    }
//        //}
//    }
//}
