using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public sealed class MemberReferance : ICodeElement, IMemberSource{

        public AbstractName Key { get; }

        public MemberReferance(AbstractName key) => this.Key = key ?? throw new ArgumentNullException(nameof(key));

        public MemberReferance(string key) : this( new ExplicitName(key) ) { }
        
        public override bool Equals(object obj)
        {
            return obj is MemberReferance referance && referance != null &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, referance.Key);
        }

        public override int GetHashCode() => 249886028 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
        public ITypeDefinition<IScope> ReturnType(ScopeStack scope)
        {
            return scope
                .GetMember(Key)
                .Type
                .GetTypeDefinitionOrThrow(scope);
        }

        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack)
        {
            return scopeStack.GetMember(Key);
        }
    }

    public sealed class TypeReferance : ICodeElement, ITypeSource
    {
        public TypeReferance(IEnumerable<AbstractName> names) => Names = names ?? throw new ArgumentNullException(nameof(names));
        public TypeReferance(string name) : this(new ExplicitName(name).ToArray()) { }

        public IEnumerable<AbstractName> Names { get; }

        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) => RootScope.TypeType;

        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition<IScope> typeDefinition) {
            typeDefinition = scope.GetType(Names);
            return true;
        }
    }
}
