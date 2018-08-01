using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    // you can totally have anonymous types...
    public sealed class TypeDefinition: IReferanced<TypeName>
    {
        public IReadOnlyList<MemberDefinition> MemberDefinitions { get; }
        public TypeName Key { get;}
    }
}
