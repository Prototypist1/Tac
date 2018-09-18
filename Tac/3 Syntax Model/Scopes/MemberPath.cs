using System;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public class MemberPath : ICodeElement
    {
        public MemberPath(int scopesUp, MemberDefinition[] memberDefinitions)
        {
            ScopesUp = scopesUp;
            MemberDefinitions = memberDefinitions ?? throw new ArgumentNullException(nameof(memberDefinitions));
        }

        public int ScopesUp { get; }
        public MemberDefinition[] MemberDefinitions { get; }
    }
}