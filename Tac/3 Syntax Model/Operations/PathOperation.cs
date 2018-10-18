using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class PathOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public PathOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            // should this check to see if the left contains the member defined on the rhs?
            return right.Cast<MemberDefinition>();
        }
    }


    public class PathOperationMaker : IOperationMaker<PathOperation>
    {
        public PathOperationMaker( BinaryOperation.Make<PathOperation> make
            )
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }
        
        private BinaryOperation.Make<PathOperation> Make { get; }

        public IResult<IPopulateScope<PathOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation("."), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ExpectPathPart(left.GetReturnType(matchingContext.Builders)).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<PathOperation>(left, right, Make));
            }

            return ResultExtension.Bad<IPopulateScope<PathOperation>>();
        }

    }
}
