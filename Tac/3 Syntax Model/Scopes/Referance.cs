using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class ReferanceOrMemberDef : ICodeElement{
        public ReferanceOrMemberDef(Referance referance, MemberDefinition memberDefinition)
        {
            Referance = referance ?? throw new ArgumentNullException(nameof(referance));
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public Referance Referance { get; }
        public MemberDefinition MemberDefinition { get; }

    }

    public class Referance: ICodeElement
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

        public IReferanced GetOrThrow(IScope compilation)
        {
            if (compilation.TryGet<IReferanced>(key, out var referanced))
            {
                return referanced;
            }
            else {
                throw new Exception($"{key} not found");
            }
        }
    }

    public class Referance<TReferanced>: Referance
        where TReferanced : IReferanced
    {
        public Referance(NamePath key) : base(key)
        {
        }

        public override bool Equals(object obj) => obj is Referance<TReferanced> && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public new TReferanced GetOrThrow(IScope compilation) {
            if (compilation.TryGet<TReferanced>(key, out var referanced))
            {
                return referanced;
            }
            else
            {
                throw new Exception($"{key} not found");
            }
        }
    }
}
