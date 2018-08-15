using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // you can totally have anonymous types...
    // that is why we have anonymous keys...
    public sealed class TypeDefinition: IScoped<ObjectScope>, IReferanced
    {
        public TypeDefinition(AbstractName key) => Key = key ?? throw new ArgumentNullException(nameof(key));

        public AbstractName Key { get; }

        public ObjectScope Scope { get; } = new ObjectScope();

        public override bool Equals(object obj)
        {
            var definition = obj as TypeDefinition;
            return definition != null &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<ObjectScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = -1628597129;
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObjectScope>.Default.GetHashCode(Scope);
            return hashCode;
        }
    }
    
}
