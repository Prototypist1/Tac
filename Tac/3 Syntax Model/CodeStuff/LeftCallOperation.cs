using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class NextCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public NextCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override bool Equals(object obj) => obj is NextCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition<IScope> ReturnType(ScopeStack scope)
        {
            if (right is MemberReferance referance)
            {
                var member = scope.GetMember(referance.Key);

                return GetTypeDefinition(member.Type.GetTypeDefinitionOrThrow(scope));
            }

            if (right is ITypeDefinition<IScope> rightTypeDef)
            {
                return GetTypeDefinition(rightTypeDef);
            }

            return GetTypeDefinition(right.ReturnType(scope));
            
            ITypeDefinition<IScope> GetTypeDefinition(ITypeDefinition<IScope> typeDefinition)
            {
                if (typeDefinition is MethodDefinition methodDefinition)
                {
                    return methodDefinition.OutputType.GetTypeDefinitionOrThrow(scope);
                }

                if (typeDefinition is ImplementationDefinition implementationDefinition)
                {
                    return implementationDefinition.OutputType.GetTypeDefinitionOrThrow(scope);
                }

                throw new Exception($"{typeDefinition} is expected to a method or implementation");
            }
        }
    }
}
