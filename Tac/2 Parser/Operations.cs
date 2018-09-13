using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{
    public class Operations
    {

        public Dictionary<string, Func<object, object, ICodeElement>> BinaryOperations { get; }
        public Dictionary<string, Func<object, ICodeElement>> NextOperations { get; }
        public Dictionary<string, Func<object, ICodeElement>> LastOperations { get; }
        public Dictionary<string, Func<ICodeElement>> ConstantOperations { get; }

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
                new Dictionary<string, Func<object, object, ICodeElement>>
                {
                    {"plus", (last, next) => new AddOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"minus", (last, next) => new SubtractOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"times", (last, next) => new MultiplyOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"if-true", (last, next) => new IfTrueOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"else", (last, next) => new ElseOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"less-than", (last, next) => new LessThanOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"next-call", (last, next) => new NextCallOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                    {"assign", (last, next) => {
                            if (next is ImplicitlyTypedMemberDefinition implicitlyTypedMember){
                                var memberDef = implicitlyTypedMember.MakeMemberDefinition(new ImplicitTypeReferance(last.Cast<ICodeElement>()));
                                return new AssignOperation(last.Cast<ICodeElement>(),memberDef);
                            }
                            return new AssignOperation(last.Cast<ICodeElement>(),next.Cast<IMemberSource>());
                        }
                    },
                    {"last-call", (last, next) => new LastCallOperation(last.Cast<ICodeElement>(),next.Cast<ICodeElement>()) },
                },
                new Dictionary<string, Func<object, ICodeElement>>
                {
                },
                new Dictionary<string, Func<object, ICodeElement>>
                {
                    {"return", (last) => new ReturnOperation(last.Cast<ICodeElement>()) },
                },
                new Dictionary<string, Func<ICodeElement>>
                {
                });
        });

        public Operations(
            Dictionary<string, Func<object, object, ICodeElement>> binaryOperations,
            Dictionary<string, Func<object, ICodeElement>> nextOperations,
            Dictionary<string, Func<object, ICodeElement>> lastOperations,
            Dictionary<string, Func<ICodeElement>> constantOperations)
        {
            BinaryOperations = binaryOperations ?? throw new ArgumentNullException(nameof(binaryOperations));
            NextOperations = nextOperations ?? throw new ArgumentNullException(nameof(nextOperations));
            LastOperations = lastOperations ?? throw new ArgumentNullException(nameof(lastOperations));
            ConstantOperations = constantOperations ?? throw new ArgumentNullException(nameof(constantOperations));
        }
    }

}
