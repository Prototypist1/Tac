using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public interface INextCallOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    public class WeakNextCallOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {

        public const string Identifier = ">";

        public WeakNextCallOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
        {
            return right.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
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

    public interface ILastCallOperation : IBinaryOperation<ICodeElement, ICodeElement>
    {
    }

    public class WeakLastCallOperation : BinaryOperation<IWeakCodeElement, IWeakCodeElement>
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IWeakCodeElement left, IWeakCodeElement right) : base(left, right)
        {
        }

        public override IWeakReturnable Returns()
        {
            return left.Unwrap<WeakMethodDefinition>().OutputType.GetValue();
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
        public static T Unwrap<T>(this IWeakCodeElement codeElement) where T:IWeakReturnable {
            if (codeElement.Returns() is WeakMemberDefinition member && member.Type.GetValue() is T t) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
