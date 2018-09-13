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

        public Dictionary<string, Func<object, object, object>> BinaryOperations { get; }
        public Dictionary<string, Func<object, object>> NextOperations { get; }
        public Dictionary<string, Func<object, object>> LastOperations { get; }
        public Dictionary<string, Func<object>> ConstantOperations { get; }

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
                new Dictionary<string, Func<object, object, object>>
                {
                    {"plus", (last, next) => new AddOperation(last,next) },
                    {"minus", (last, next) => new SubtractOperation(last,next) },
                    {"times", (last, next) => new MultiplyOperation(last,next) },
                    {"if-true", (last, next) => new IfTrueOperation(last,next) },
                    {"else", (last, next) => new ElseOperation(last,next) },
                    {"less-than", (last, next) => new LessThanOperation(last,next) },
                    {"next-call", (last, next) => new NextCallOperation(last,next) },
                    {"assign", (last, next) => {
                            if (next is ImplicitlyTypedMemberDefinition implicitlyTypedMember){
                                var memberDef = implicitlyTypedMember.MakeMemberDefinition(new ImplicitTypeReferance(last));
                                return new AssignOperation(last,memberDef);
                            }
                            return new AssignOperation(last,next.Cast<IMemberSource>());
                        }
                    },
                    {"last-call", (last, next) => new LastCallOperation(last,next) },
                },
                new Dictionary<string, Func<object, object>>
                {
                },
                new Dictionary<string, Func<object, object>>
                {
                    {"return", (last) => new ReturnOperation(last) },
                },
                new Dictionary<string, Func<object>>
                {
                });
        });

        public Operations(
            Dictionary<string, Func<object, object, object>> binaryOperations,
            Dictionary<string, Func<object, object>> nextOperations,
            Dictionary<string, Func<object, object>> lastOperations,
            Dictionary<string, Func<object>> constantOperations)
        {
            BinaryOperations = binaryOperations ?? throw new ArgumentNullException(nameof(binaryOperations));
            NextOperations = nextOperations ?? throw new ArgumentNullException(nameof(nextOperations));
            LastOperations = lastOperations ?? throw new ArgumentNullException(nameof(lastOperations));
            ConstantOperations = constantOperations ?? throw new ArgumentNullException(nameof(constantOperations));
        }
    }

}
