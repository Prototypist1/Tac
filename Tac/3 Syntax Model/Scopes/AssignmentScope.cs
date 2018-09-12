using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class AssignmentScope : IScope
    {
        public AssignmentScope(ICodeElement leftSide)
        {
            LeftSide = leftSide ?? throw new ArgumentNullException(nameof(leftSide));
        }

        public ICodeElement LeftSide { get; }
        
        public bool TryGet(ImplicitTypeReferance key, out Func<ScopeStack, ITypeDefinition<IScope>> item) {
            item = LeftSide.ReturnType;
            return true;
        }

        public bool TryGetMember(AbstractMemberName name, bool staticOnly, out MemberDefinition member) {
            member = default;
            return false;
        }
        public bool TryGetType(AbstractMemberName name, out TypeDefinition type) {
            type = default;
            return false;
        }
    }
}