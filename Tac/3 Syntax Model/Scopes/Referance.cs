using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class ReferanceOrMemberDef {
        public ReferanceOrMemberDef(Referance referance, AbstractMemberDefinition memberDefinition)
        {
            Referance = referance ?? throw new ArgumentNullException(nameof(referance));
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public Referance Referance { get; }
        public AbstractMemberDefinition MemberDefinition { get; }

        // smells 
        public ITypeDefinition ReturnType(IScope scope) => throw new NotImplementedException();
    }
    
    public sealed class Referance : ICodeElement{
        public readonly NamePath key;

        public Referance(NamePath key) => this.key = key ?? throw new ArgumentNullException(nameof(key));

        public Referance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) })) { }
        
        public override bool Equals(object obj)
        {
            return obj is Referance referance && referance != null &&
                   EqualityComparer<NamePath>.Default.Equals(key, referance.key);
        }

        public override int GetHashCode() => 249886028 + EqualityComparer<NamePath>.Default.GetHashCode(key);
        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            if (scope.TryGet(key.names, out var res))
            {
                return res.ReturnType(scope);
            }

            throw new Exception("Referance Not resolved");
        }
    }
}
