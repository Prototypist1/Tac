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

    public class WeakNextCallOperation : BinaryOperation<ICodeElement, ICodeElement>, INextCallOperation
    {

        public const string Identifier = ">";

        public WeakNextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return Right.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
        }
    }

    public class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation>
    {
        public NextCallOperationMaker() : base(WeakNextCallOperation.Identifier, (l,r)=> new WeakNextCallOperation(l,r), new NextCallConverter())
        {
        }

        private class NextCallConverter : IConverter<WeakNextCallOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakNextCallOperation co)
            {
                return context.NextCallOperation(co);
            }
        }
    }

    public class WeakLastCallOperation : BinaryOperation<ICodeElement, ICodeElement>, ILastCallOperation
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IType Returns()
        {
            return Left.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
        }
    }

    public class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation>
    {
        public LastCallOperationMaker() : base(WeakLastCallOperation.Identifier, (l,r)=>new WeakLastCallOperation(l,r), new LastCallConverter())
        {
        }

        private class LastCallConverter : IConverter<WeakLastCallOperation>
        {
            public T Convert<T>(IOpenBoxesContext<T> context, WeakLastCallOperation co)
            {
                return context.LastCallOperation(co);
            }
        }
    }
    
    public static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement) where T:IType {
            if (codeElement.Returns() is WeakMemberDefinition member && member.Type.GetValue() is T t) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
