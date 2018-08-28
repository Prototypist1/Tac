using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class ReferanceOrMemberDef {
        public ReferanceOrMemberDef(IReferance referance, MemberDefinition memberDefinition)
        {
            Referance = referance ?? throw new ArgumentNullException(nameof(referance));
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IReferance Referance { get; }
        public MemberDefinition MemberDefinition { get; }

        // smells 
        public ITypeDefinition ReturnType(IScope scope) => throw new NotImplementedException();
    }

    public interface IReferance : ICodeElement {

    }

    public sealed class Referance: IReferance 
    {
        public readonly NamePath key;

        public Referance(NamePath key) => this.key = key ?? throw new ArgumentNullException(nameof(key));

        public Referance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) })) { }
        
        public override bool Equals(object obj)
        {
            return obj is Referance referance && referance != null &&
                   EqualityComparer<NamePath>.Default.Equals(key, referance.key);
        }

        public override int GetHashCode() => 249886028 + EqualityComparer<NamePath>.Default.GetHashCode(key);
        public ITypeDefinition ReturnType(IScope scope)
        {
            if (scope.TryGet(this, out var res))
            {
                return res.ReturnType(scope);
            }

            throw new Exception("Referance Not resolved");
        }
    }
}
