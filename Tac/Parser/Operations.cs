using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operations
    {

        public Dictionary<string, Func<ParseContext, ParseContext, CodeElement>> BinaryOperations { get; }
        public Dictionary<string, Func<ParseContext, CodeElement>> NextOperations { get; }
        public Dictionary<string, Func<ParseContext, CodeElement>> LastOperations { get; }
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
                new Dictionary<string, Func<ParseContext, ParseContext, CodeElement>>
                {
                    {"plus", (last, next) => new AddOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"minus", (last, next) => new SubtractOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"times", (last, next) => new MultiplyOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"if-true", (last, next) => new IfTrueOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"less-then", (last, next) => new LessThanOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"next-call", (last, next) => new NextCallOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"assign", (last, next) => new AssignOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    {"last-call", (last, next) => new LastCallOperation(last.ToCodeElement(),next.ToCodeElement()) },
                    { "is-static", (last, next) => new IsStaticOperation(last.ToCodeElement(),next.ToCodeElement()) },
                },
                new Dictionary<string, Func<ParseContext, CodeElement>>
                {
                    {"var", (next) => new VarOperation(next.ToCodeElement()) },
                },
                new Dictionary<string, Func<ParseContext, CodeElement>>
                {
                    {"return", (last) => new ReturnOperation(last.ToCodeElement()) },
                }, 
                new Dictionary<string, Func<CodeElement>>
                {
                });
        });

        public Operations(Dictionary<string, Func<ParseContext, ParseContext, CodeElement>> binaryOperations, Dictionary<string, Func<ParseContext, CodeElement>> nextOperations, Dictionary<string, Func<ParseContext, CodeElement>> lastOperations, Dictionary<string, Func<CodeElement>> constantOperations)
        {
            BinaryOperations = binaryOperations ?? throw new ArgumentNullException(nameof(binaryOperations));
            NextOperations = nextOperations ?? throw new ArgumentNullException(nameof(nextOperations));
            LastOperations = lastOperations ?? throw new ArgumentNullException(nameof(lastOperations));
            ConstantOperations = constantOperations ?? throw new ArgumentNullException(nameof(constantOperations));
        }
    }

}
