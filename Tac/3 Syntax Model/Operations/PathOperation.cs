using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    internal class WeakPathOperation : BinaryOperation<ICodeElement, ICodeElement>, IPathOperation
    {
        public const string Identifier = ".";

        public WeakPathOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.PathOperation(this);
        }
        
        public override IType Returns()
        {
            // should this check to see if the left contains the member defined on the rhs?
            return Right.Cast<WeakMemberReferance>();
        }
    }


    internal class PathOperationMaker : IOperationMaker<WeakPathOperation>
    {
        public IResult<IPopulateScope<WeakPathOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(WeakPathOperation.Identifier), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ExpectPathPart(left.GetReturnType()).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<WeakPathOperation>(left, right, (l,r)=> new WeakPathOperation(l,r)));
            }

            return ResultExtension.Bad<IPopulateScope<WeakPathOperation>>();
        }
    }
}
