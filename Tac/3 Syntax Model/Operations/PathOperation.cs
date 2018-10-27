using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class WeakPathOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = ".";

        public WeakPathOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
        {
            // should this check to see if the left contains the member defined on the rhs?
            return right.Cast<WeakMemberReferance>();
        }
    }


    public class PathOperationMaker : IOperationMaker<WeakPathOperation>
    {
        public IResult<IPopulateScope<WeakPathOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(WeakPathOperation.Identifier), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ExpectPathPart(left.GetReturnType()).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<WeakPathOperation>(left, right, (l,r)=> new WeakPathOperation(l,r), new Converter()));
            }

            return ResultExtension.Bad<IPopulateScope<WeakPathOperation>>();
        }


        private class Converter : IConverter<WeakPathOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakPathOperation co)
            {
                return context.PathOperation(co);
            }
        }

    }
}
