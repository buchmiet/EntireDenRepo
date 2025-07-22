//using Microsoft.CodeAnalysis;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.CodeAnalysis.Text;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Trier
//{

//    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
//    public class GenerateTryMethodAttribute : Attribute { }

//    public interface ITryMethodProvider { }

//    [Generator]
//    public class TryMethodGenerator : ISourceGenerator
//    {
//        public void Initialize(GeneratorInitializationContext context)
//        {
//            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
//        }

//        public void Execute(GeneratorExecutionContext context)
//        {
//            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
//                return;

//            foreach (var classDeclaration in receiver.CandidateClasses)
//            {
//                string classNamespace = GetNamespace(classDeclaration);
//                string className = classDeclaration.Identifier.Text;

//                string generatedCode = $@"
//namespace {classNamespace}
//{{
//    public partial class {className}
//    {{
//        private Func<Func<TAnswer>, out TAnswer, bool> _tryImplementation;

//        public void PrepareTry(IAnswerService answerService)
//        {{
//            _tryImplementation = answerService.Try;
//        }}

//        protected bool Try<TAnswer>(Func<TAnswer> action, out TAnswer answer) where TAnswer : IAnswerBase
//        {{
//            if (_tryImplementation == null)
//            {{
//                throw new InvalidOperationException(""PrepareTry must be called before using Try"");
//            }}
//            return _tryImplementation(action, out answer);
//        }}
//    }}
//}}";

//                context.AddSource($"{className}.g.cs", SourceText.From(generatedCode, Encoding.UTF8));
//            }
//        }

//        private class SyntaxReceiver : ISyntaxContextReceiver
//        {
//            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

//            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
//            {
//                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
//                {
//                    var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
//                    if (symbol != null)
//                    {
//                        bool hasAttribute = symbol.GetAttributes()
//                            .Any(ad => ad.AttributeClass?.Name == "GenerateTryMethodAttribute");

//                        bool implementsInterface = symbol.AllInterfaces
//                            .Any(i => i.Name == "ITryMethodProvider");

//                        if (hasAttribute || implementsInterface)
//                        {
//                            CandidateClasses.Add(classDeclarationSyntax);
//                        }
//                    }
//                }
//            }
//        }

//        private string GetNamespace(ClassDeclarationSyntax classDeclaration)
//        {
//            // Przechodzimy w górę drzewa składni, szukając deklaracji przestrzeni nazw
//            SyntaxNode parent = classDeclaration.Parent;
//            while (parent != null && !(parent is NamespaceDeclarationSyntax))
//            {
//                parent = parent.Parent;
//            }

//            if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
//            {
//                return namespaceDeclaration.Name.ToString();
//            }

//            // Jeśli nie znaleziono przestrzeni nazw, zwracamy pusty string
//            return string.Empty;
//        }
//    }
//}
