using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class TypeDefinition
    {
        public IReadOnlyList<MemberDefinition> MemberDefinitions { get; }
        public TypeName TypeNameDefinition { get;}
    }
}
