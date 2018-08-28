using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operations
    {

        public Dictionary<string, Func<ICodeElement, ICodeElement, IScope, ICodeElement>> BinaryOperations { get; }
        public Dictionary<string, Func<ICodeElement, IScope, ICodeElement>> NextOperations { get; }
        public Dictionary<string, Func<ICodeElement, IScope, ICodeElement>> LastOperations { get; }
        public Dictionary<string, Func<IScope, ICodeElement>> ConstantOperations { get; }

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
                new Dictionary<string, Func<ICodeElement, ICodeElement,IScope, ICodeElement>>
                {
                    {"plus", (last, next,scope) => new AddOperation(last,next) },
                    {"minus", (last, next,scope) => new SubtractOperation(last,next) },
                    {"times", (last, next,scope) => new MultiplyOperation(last,next) },
                    {"if-true", (last, next,scope) => new IfTrueOperation(last,next) },
                    {"else", (last, next,scope) => new ElseOperation(last,next) },
                    {"less-than", (last, next,scope) => new LessThanOperation(last,next) },
                    {"next-call", (last, next,scope) => new NextCallOperation(last,next) },
                    {"assign", (last, next,scope) => new AssignOperation(last, next,new AssignmentScope(scope)) },
                    {"last-call", (last, next,scope) => new LastCallOperation(last,next) },
                },
                new Dictionary<string, Func<ICodeElement, IScope, ICodeElement>>
                {
                },
                new Dictionary<string, Func<ICodeElement, IScope, ICodeElement>>
                {
                    {"return", (last,scope) => new ReturnOperation(last) },
                }, 
                new Dictionary<string, Func<IScope,ICodeElement>>
                {
                });
        });

        public Operations(
            Dictionary<string, Func<ICodeElement, ICodeElement, IScope, ICodeElement>> binaryOperations, 
            Dictionary<string, Func<ICodeElement, IScope, ICodeElement>> nextOperations, 
            Dictionary<string, Func<ICodeElement, IScope, ICodeElement>> lastOperations, 
            Dictionary<string, Func<IScope, ICodeElement>> constantOperations)
        {
            BinaryOperations = binaryOperations ?? throw new ArgumentNullException(nameof(binaryOperations));
            NextOperations = nextOperations ?? throw new ArgumentNullException(nameof(nextOperations));
            LastOperations = lastOperations ?? throw new ArgumentNullException(nameof(lastOperations));
            ConstantOperations = constantOperations ?? throw new ArgumentNullException(nameof(constantOperations));
        }
    }

}
