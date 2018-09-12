using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ModuleDefinition : IScoped, ICodeElement 
    {
        public ModuleDefinition(AbstractMemberName key, IScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public AbstractMemberName Key { get; }

        public IScope Scope { get; }
        
        public override bool Equals(object obj)
        {
            return obj is ModuleDefinition definition && definition != null &&
                   EqualityComparer<AbstractMemberName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<IScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = -1628597129;
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractMemberName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<IScope>.Default.GetHashCode(Scope);
            return hashCode;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) => throw new NotImplementedException();
    }
}
