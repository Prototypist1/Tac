using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    // you can totally have anonymous types...
    public sealed class TypeDefinition: IScoped<ObjectScope>, IReferanced
    {
        public TypeDefinition(AbstractName key) => Key = key ?? throw new ArgumentNullException(nameof(key));

        public AbstractName Key { get; }

        public ObjectScope Scope { get; } = new ObjectScope();
    }
    
}
