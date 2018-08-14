using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class Referance: CodeElement
    {
        public readonly NamePath key;

        public Referance(NamePath key) => this.key = key ?? throw new ArgumentNullException(nameof(key));

        public IReferanced GetOrThrow(IScope compilation)
        {
            return compilation.Get<IReferanced>(key);
        }
    }

    public class Referance<TReferanced>: Referance
        where TReferanced : IReferanced
    {
        public Referance(NamePath key) : base(key)
        {
        }

        public new TReferanced GetOrThrow(IScope compilation) {
            return compilation.Get<TReferanced>(key);
        }
    }

    public class TypeReferance : Referance<TypeDefinition>
    {
        public TypeReferance(NamePath key) : base(key)
        {
        }
    }

    public class StaticMethodReferance : Referance<StaticMethodDefinition>
    {
        public StaticMethodReferance(NamePath key) : base(key)
        {
        }
    }

    public class MemberReferance : Referance<MemberDefinition>
    {
        public MemberReferance(NamePath key) : base(key)
        {
        }
    }

    public class LocalReferance : Referance<LocalDefinition>
    {
        public LocalReferance(NamePath key) : base(key)
        {
        }
    }

    public class ParameterReferance : Referance<ParameterDefinition>
    {
        public ParameterReferance(NamePath key) : base(key)
        {
        }
    }
}
