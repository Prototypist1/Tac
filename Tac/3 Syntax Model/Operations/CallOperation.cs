using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{


    internal class NextCallSymbols : ISymbols
    {
        public string Symbols => ">";
    }

    internal class WeakNextCallOperation : BinaryOperation<ICodeElement, ICodeElement>, INextCallOperation
    {
        public WeakNextCallOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }


        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.NextCallOperation(this);
        }


        public override IVarifiableType Returns()
        {
            return Right.GetOrThrow().Unwrap<WeakMethodDefinition>().OutputType.IfIs(x => x.TypeDefinition).IfIs(x => x.GetValue());
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation>
    {
        public NextCallOperationMaker() : base(new NextCallSymbols(), (l,r)=> Possibly.Is( new WeakNextCallOperation(l,r)))
        {
        }
    }


    internal class LastCallSymbols : ISymbols
    {
        public string Symbols => "<";
    }

    internal class WeakLastCallOperation : BinaryOperation<ICodeElement, ICodeElement>, ILastCallOperation
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IIsPossibly<ICodeElement> left, IIsPossibly<ICodeElement> right) : base(left, right)
        {
        }


        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.LastCallOperation(this);
        }


        public override IVarifiableType Returns()
        {
            return Left.GetOrThrow().Unwrap<WeakMethodDefinition>().OutputType.IfIs(x=>x.TypeDefinition).IfIs(x=>x.GetValue());
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation>
    {
        public LastCallOperationMaker() : base(new LastCallSymbols(), (l,r)=>Possibly.Is( new WeakLastCallOperation(l,r)))
        {
        }
    }

    internal static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement) where T:IVarifiableType {
            if (codeElement.Returns().Is< WeakMemberDefinition>(out var member) && 
                member.Type.IsDefinately(out var yes, out var _) &&  
                yes.Value.TypeDefinition.IsDefinately(out var yes2, out var _) &&
                yes2.Value.GetValue().IsDefinately(out var yes3, out var _) &&
                yes3.Is<T>(out var t)) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
