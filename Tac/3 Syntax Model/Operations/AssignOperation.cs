using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public interface IAssignOperation : IBinaryOperation<IMemberReferance, ICodeElement>
    {
    }

    public class WeakAssignOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "=:";
        
        public WeakAssignOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return left.Returns(elementBuilders);
        }
    }
    
    public class AssignOperationMaker : IOperationMaker<WeakAssignOperation>
    {
        public AssignOperationMaker(BinaryOperation.Make<WeakAssignOperation> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }
        
        private BinaryOperation.Make<WeakAssignOperation> Make { get; }

        public IResult<IPopulateScope<WeakAssignOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(WeakAssignOperation.Identifier), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.AcceptImplicit(left.GetReturnType(matchingContext.Builders)).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<WeakAssignOperation>(left, right, Make, new AssignConverter()));
            }

            return ResultExtension.Bad<IPopulateScope<WeakAssignOperation>>();
        }


        private class AssignConverter : IConverter<WeakAssignOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakAssignOperation co)
            {
                return context.AssignOperation(co);
            }
        }

    }

}
