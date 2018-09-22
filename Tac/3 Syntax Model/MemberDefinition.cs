using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 

    public class MemberDefinition 
    {
        public MemberDefinition(bool readOnly, ExplicitMemberName key, ITypeDefinition type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ITypeDefinition Type { get; }
        public bool ReadOnly { get; }
        public ExplicitMemberName Key { get; }
        
        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack)
        {
            return this;
        }
    }
}