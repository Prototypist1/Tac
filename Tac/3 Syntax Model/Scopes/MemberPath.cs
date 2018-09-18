using System;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public class MemberPath : ICodeElement
    {
        public MemberPath(int scopesUp, MemberDefinition memberDefinition)
        {
            ScopesUp = scopesUp;
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public int ScopesUp { get; }
        public MemberDefinition MemberDefinition { get; }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return MemberDefinition.ReturnType(scope);
        }
    }
}