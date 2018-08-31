using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ModuleDefinition : IReferanced, IScoped<StaticScope>, ICodeElement 
    {
        public ModuleDefinition(AbstractName key, StaticScope scope, IEnumerable<ICodeElement> staticInitialization)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public AbstractName Key { get; }

        public StaticScope Scope { get; }
        
        public override bool Equals(object obj)
        {
            return obj is ModuleDefinition definition && definition != null &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<StaticScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = -1628597129;
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<StaticScope>.Default.GetHashCode(Scope);
            return hashCode;
        }

        public ITypeDefinition<IScope> ReturnType(ScopeScope scope) => throw new NotImplementedException();
    }
}
