using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public interface ITypeDefinition: ICodeElement
    {

    }

    // you can totally have anonymous types...
    // that is why we have anonymous keys...
    public class TypeDefinition: IScoped<ObjectScope>, IReferanced,  ITypeDefinition
    {
        public TypeDefinition(AbstractName key, IScope enclosingScope)
        {
            Scope = new ObjectScope();
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AbstractName Key { get; }

        public ObjectScope Scope { get; }

        public override bool Equals(object obj)
        {
            return obj is TypeDefinition definition && definition != null &&
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


        public ITypeDefinition ReturnType(ScopeStack scope) {
            // TODO
            // I had this as returning "this"
            // that is not really true tho,
            // this returns a type...
        }
    }
    
}
