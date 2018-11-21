using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
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


    internal class NextCallSymbols : ISymbols
    {
        public string Symbols => ">";
    }

    internal class WeakNextCallOperation : BinaryOperation<ICodeElement, ICodeElement>, INextCallOperation
    {
        public WeakNextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.NextCallOperation(this);
        }


        public override IVarifiableType Returns()
        {
            return Right.Unwrap<WeakMethodDefinition>().OutputType.TypeDefinition.GetValue();
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation>
    {
        public NextCallOperationMaker() : base(new NextCallSymbols(), (l,r)=> new WeakNextCallOperation(l,r))
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

        public WeakLastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.LastCallOperation(this);
        }


        public override IVarifiableType Returns()
        {
            return Left.Unwrap<WeakMethodDefinition>().OutputType.TypeDefinition.GetValue();
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation>
    {
        public LastCallOperationMaker() : base(new LastCallSymbols(), (l,r)=>new WeakLastCallOperation(l,r))
        {
        }
    }

    internal static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement) where T:IVarifiableType {
            if (codeElement.Returns() is WeakMemberDefinition member && member.Type.TypeDefinition.GetValue() is T t) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
