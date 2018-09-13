using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override bool Equals(object obj) => obj is NextCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return CallHelp.GetTargetReturnType(right, scope);
        }
    }

    public static class CallHelp{
        public static ITypeDefinition GetTargetReturnType(ICodeElement element, ScopeStack scope) {

            if (element is IMemberSource referance)
            {
                var member = referance.GetMemberDefinition(scope);

                return GetTypeDefinition(member.Type.GetTypeDefinition(scope));
            }

            if (element is ITypeDefinition rightTypeDef)
            {
                return GetTypeDefinition(rightTypeDef);
            }

            return GetTypeDefinition(element.ReturnType(scope));

            ITypeDefinition GetTypeDefinition(ITypeDefinition typeDefinition)
            {
                if (typeDefinition is MethodDefinition methodDefinition)
                {
                    return methodDefinition.OutputType.GetTypeDefinition(scope);
                }

                if (typeDefinition is ImplementationDefinition implementationDefinition)
                {
                    return implementationDefinition.OutputType.GetTypeDefinition(scope);
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

        public override bool Equals(object obj) => obj is LastCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope)
        {
            return CallHelp.GetTargetReturnType(left, scope);
        }
    }
}
