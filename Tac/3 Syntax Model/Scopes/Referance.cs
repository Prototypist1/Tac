using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public sealed class MemberReferance : ICodeElement, IFeildOrMemberSource{
        public readonly NamePath key;

        public MemberReferance(NamePath key) => this.key = key ?? throw new ArgumentNullException(nameof(key));

        public MemberReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) })) { }
        
        public override bool Equals(object obj)
        {
            return obj is MemberReferance referance && referance != null &&
                   EqualityComparer<NamePath>.Default.Equals(key, referance.key);
        }

        public override int GetHashCode() => 249886028 + EqualityComparer<NamePath>.Default.GetHashCode(key);
        public ITypeDefinition<IScope> ReturnType(ScopeScope scope)
        {
            if (scope.TryGet(key.names, out var res))
            {
                return res.ReturnType(scope);
            }

            throw new Exception("Referance Not resolved");
        }
    }
}
