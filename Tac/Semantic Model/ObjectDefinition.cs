using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class ObjectDefinition
    {
        public IReadOnlyList<MemberDefinition> MemberDefinitions { get; }
    }
}
