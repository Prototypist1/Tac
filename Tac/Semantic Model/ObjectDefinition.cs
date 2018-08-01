using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Semantic_Model
{
    public sealed class ObjectDefinition
    {
        // I think member defintions are really refs...??
        public IReadOnlyList<MemberDefinition> MemberDefinitions { get; }
    }
}
