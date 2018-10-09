using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class PathOperation : BinaryOperation<Member, PathPart>
    {
        public PathOperation(Member left, PathPart right) : base(left, right)
        {
        }

        public override IReturnable ReturnType()
        {
            if (!left.Cast<IScoped>().Scope.TryGetMember(right.MemberDefinition.GetValue().Key,false,out var check)){
                throw new Exception("Member should be defined");
            }

            if (!check.GetValue().Type.GetValue().Key.Equals(right.MemberDefinition.GetValue().Type.GetValue().Key)) {
                throw new Exception("we have two ways to get to the type, they better have the same value");
            }
            
            return right.MemberDefinition.GetValue().Type.GetValue();
        }
    }


    public class PathOperationMaker : IOperationMaker<PathOperation>
    {
        public PathOperationMaker(string name, Func<ICodeElement, ICodeElement, PathOperation> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Name { get; }
        private Func<ICodeElement, ICodeElement, PathOperation> Make { get; }

        public IResult<IPopulateScope<PathOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ExpectPathPart().ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<PathOperation>(left, right, Make));
            }

            return ResultExtension.Bad<IPopulateScope<PathOperation>>();
        }

    }
}
