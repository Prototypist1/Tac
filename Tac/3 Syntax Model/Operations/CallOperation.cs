﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
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

        public override IReturnable ReturnType()
        {
            return right.Unwrap<MethodDefinition>().OutputType.GetValue();
        }
    }

    public class NextCallOperationMaker : BinaryOperationMaker<NextCallOperation>
    {
        public NextCallOperationMaker(Func<ICodeElement, ICodeElement, NextCallOperation> make) : base(">", make)
        {
        }
    }

    public class LastCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IReturnable ReturnType()
        {
            return left.Unwrap<MethodDefinition>().OutputType.GetValue();
        }
    }

    public class LastCallOperationMaker : BinaryOperationMaker<LastCallOperation>
    {
        public LastCallOperationMaker(Func<ICodeElement, ICodeElement, LastCallOperation> make) : base("<", make)
        {
        }
    }

    public static class MemberUnwrapper{
        public static T Unwrap<T>(this ICodeElement codeElement) {
            if (codeElement.ReturnType() is Member member && member.MemberDefinition.GetValue() is T t) {
                return t;
            }
            return codeElement.ReturnType().Cast<T>();
        }
    }
}