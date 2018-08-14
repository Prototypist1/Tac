using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operations
    {

        public Dictionary<string, Func<CodeElement, CodeElement, CodeElement>> BinaryOperations { get; }
        public Dictionary<string, Func<CodeElement, CodeElement>> NextOperations { get; }
        public Dictionary<string, Func<CodeElement, CodeElement>> LastOperations { get; }
        public Dictionary<string, Func<CodeElement>> ConstantOperations { get; }

        public IEnumerable<string> AllOperationKeys
        {
            get
            {
                foreach (var item in BinaryOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in NextOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in LastOperations.Keys)
                {
                    yield return item;
                }
                foreach (var item in ConstantOperations.Keys)
                {
                    yield return item;
                }
            }
        }

        public static Lazy<Operations> StandardOperations = new Lazy<Operations>(() =>
        {
            return new Operations(
                new Dictionary<string, Func<CodeElement, CodeElement, CodeElement>>
                {
                    {"plus", (last, next) => new AddOperation(last,next) },
                    {"minus", (last, next) => new SubtractOperation(last,next) },
                    {"times", (last, next) => new MultiplyOperation(last,next) },
                    {"if-true", (last, next) => new IfTrueOperation(last,next) },
                    {"else", (last, next) => new ElseOperation(last,next) },
                    {"less-then", (last, next) => new LessThanOperation(last,next) },
                    {"next-call", (last, next) => new NextCallOperation(last,next) },
                    {"assign", (last, next) => new AssignOperation(last,next) },
                    {"last-call", (last, next) => new LastCallOperation(last,next) },
                    {"is-static", (last, next) => new IsStaticOperation(last,next) },
                },
                new Dictionary<string, Func<CodeElement, CodeElement>>
                {
                },
                new Dictionary<string, Func<CodeElement, CodeElement>>
                {
                    {"return", (last) => new ReturnOperation(last) },
                }, 
                new Dictionary<string, Func<CodeElement>>
                {
                });
        });

        public Operations(
            Dictionary<string, Func<CodeElement, CodeElement, CodeElement>> binaryOperations, 
            Dictionary<string, Func<CodeElement, CodeElement>> nextOperations, 
            Dictionary<string, Func<CodeElement, CodeElement>> lastOperations, 
            Dictionary<string, Func<CodeElement>> constantOperations)
        {
            BinaryOperations = binaryOperations ?? throw new ArgumentNullException(nameof(binaryOperations));
            NextOperations = nextOperations ?? throw new ArgumentNullException(nameof(nextOperations));
            LastOperations = lastOperations ?? throw new ArgumentNullException(nameof(lastOperations));
            ConstantOperations = constantOperations ?? throw new ArgumentNullException(nameof(constantOperations));
        }
    }

}
