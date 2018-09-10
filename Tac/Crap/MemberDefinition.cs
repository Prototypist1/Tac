using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    public sealed class ExplicitTypeSource : ITypeSource
    {
        public ExplicitTypeSource(ICodeElement codeElement) => CodeElement = codeElement ?? throw new ArgumentNullException(nameof(codeElement));

        private ICodeElement CodeElement { get; }

        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition) {
            typeDefinition = CodeElement.ReturnType(scope);
            return true;
        }
    }

    public sealed class NameTypeSource : ITypeSource
    {
        public NameTypeSource(AbstractName name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public AbstractName Name { get; }

        // try is a little woerd here
        // not really the right api design
        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition)
        {
            typeDefinition= scope.GetType(Name);
            return true;
        }
    }
    
    public sealed class ConstantTypeSource : ITypeSource
    {
        public ConstantTypeSource(ITypeDefinition typeDefinition) => TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));

        private ITypeDefinition TypeDefinition { get; }

        public bool TryGetTypeDefinition(ScopeStack scope, out ITypeDefinition typeDefinition)
        {
            typeDefinition = TypeDefinition;
            return true;
        }
    }

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 

    public sealed class MemberDefinition: ICodeElement, IMemberSource
    {
        public MemberDefinition(bool readOnly, AbstractName key, ITypeSource type)
        {
            Type = type;
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public ITypeSource Type { get; }
        public bool ReadOnly { get; }
        public AbstractName Key { get; }

        public override bool Equals(object obj)
        {
            return obj is MemberDefinition definition &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<ITypeSource>.Default.Equals(Type, definition.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(Type);
            return hashCode;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) {
            if (!Type.TryGetTypeDefinition(scope, out var res)) {
                throw new Exception("Type not found");
            }

            return res;
        }

        public MemberDefinition GetMemberDefinition(ScopeStack scopeStack) => this;
    }
}