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

    internal class WeakNextCallOperation : BinaryOperation<ICodeElement, ICodeElement>, INextCallOperation
    {

        public const string Identifier = ">";

        public WeakNextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }


        public override T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.NextCallOperation(this);
        }


        public override IType Returns()
        {
            return Right.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation>
    {
        public NextCallOperationMaker() : base(WeakNextCallOperation.Identifier, (l,r)=> new WeakNextCallOperation(l,r))
        {
        }
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


        public override IType Returns()
        {
            return Left.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation>
    {
        public LastCallOperationMaker() : base(WeakLastCallOperation.Identifier, (l,r)=>new WeakLastCallOperation(l,r))
        {
        }
    }

    internal static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement) where T:IType {
            if (codeElement.Returns() is WeakMemberDefinition member && member.Type.GetValue() is T t) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
