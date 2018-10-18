using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class AssignOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public AssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return left.Returns(elementBuilders);
        }
    }
    
    public class AssignOperationMaker : IOperationMaker<AssignOperation>
    {
        public AssignOperationMaker(BinaryOperation.Make<AssignOperation> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }
        
        private BinaryOperation.Make<AssignOperation> Make { get; }

        public IResult<IPopulateScope<AssignOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(""), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.AcceptImplicit(left.GetReturnType(matchingContext.Builders)).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<AssignOperation>(left, right, Make));
            }

            return ResultExtension.Bad<IPopulateScope<AssignOperation>>();
        }

    }
}
