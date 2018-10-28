using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{

    public class WeakAssignOperation : BinaryOperation<ICodeElement, ICodeElement>, IAssignOperation
    {
        public const string Identifier = "=:";
        
        public WeakAssignOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return Left.Returns();
        }
    }
    
    public class AssignOperationMaker : IOperationMaker<WeakAssignOperation>
    {
        public AssignOperationMaker()
        {
        }
        

        public IResult<IPopulateScope<WeakAssignOperation>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(WeakAssignOperation.Identifier), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.AcceptImplicit(left.GetReturnType()).ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<WeakAssignOperation>(left, right, (l,r)=>new WeakAssignOperation(l,r), new AssignConverter()));
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
