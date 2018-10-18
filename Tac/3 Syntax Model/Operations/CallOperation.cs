using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return right.Unwrap<MethodDefinition>(elementBuilders).OutputType.GetValue();
        }
    }

    public class NextCallOperationMaker : BinaryOperationMaker<NextCallOperation>
    {
        public NextCallOperationMaker(BinaryOperation.Make<NextCallOperation> make) : base(">", make)
        {
        }
    }

    public class LastCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable Returns(IElementBuilders elementBuilders)
        {
            return left.Unwrap<MethodDefinition>(elementBuilders).OutputType.GetValue();
        }
    }

    public class LastCallOperationMaker : BinaryOperationMaker<LastCallOperation>
    {
        public LastCallOperationMaker(BinaryOperation.Make<LastCallOperation> make) : base("<", make)
        {
        }
    }

    public static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement, IElementBuilders elementBuilders) {
            if (codeElement.Returns(elementBuilders) is Member member && member.MemberDefinitions.Last().GetValue() is T t) {
                return t;
            }
            return codeElement.Returns(elementBuilders).Cast<T>();
        }
    }
}
