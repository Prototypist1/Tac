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
        
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return CallHelp.GetTargetReturnType(right, scope);
        }
    }

    public class NextCallOperationMaker : BinaryOperationMaker<NextCallOperation>
    {
        public NextCallOperationMaker(Func<ICodeElement, ICodeElement, NextCallOperation> make, IElementBuilders elementBuilders) : base(">", make, elementBuilders)
        {
        }
    }

    public static class CallHelp{
        public static ITypeDefinition GetTargetReturnType(ICodeElement element, ScopeStack scope) {

            if (element is ExplicitMemberName referance)
            {
                var member = scope.GetMemberDefinition(referance.Key);

                return GetReturnType(member.Type);
            }

            if (element is ITypeDefinition rightTypeDef)
            {
                return GetReturnType(rightTypeDef);
            }

            return GetReturnType(element.ReturnType(scope));

            ITypeDefinition GetReturnType(ITypeDefinition typeDefinition)
            {
                if (typeDefinition is MethodDefinition methodDefinition)
                {
                    return methodDefinition.OutputType;
                }

                if (typeDefinition is ImplementationDefinition implementationDefinition)
                {
                    return implementationDefinition.OutputType;
                }

                throw new Exception($"{typeDefinition} is expected to a method or implementation");
            }
        }
    }
    
    public class LastCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return CallHelp.GetTargetReturnType(left, scope);
        }
    }

    public class LastCallOperationMaker : BinaryOperationMaker<LastCallOperation>
    {
        public LastCallOperationMaker(Func<ICodeElement, ICodeElement, LastCallOperation> make, IElementBuilders elementBuilders) : base("<", make, elementBuilders)
        {
        }
    }
}
