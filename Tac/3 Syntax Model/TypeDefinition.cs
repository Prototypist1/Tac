using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public interface ITypeDefinition<out TScope>: ICodeElement, IScoped<TScope>
        where TScope: IScope
    {

    }
    
    public class TypeDefinition: IReferanced,  ITypeDefinition<ObjectScope>
    {
        public TypeDefinition(AbstractName key)
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
        
        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) {
            return RootScope.TypeType;
        }
    }

    public class GenericTypeDefinition : TypeDefinition {

        public GenericTypeDefinition(AbstractName key,int typeCount): base(key)
        {
            TypeCount = typeCount;
        }

        public int TypeCount { get; }

        public override bool Equals(object obj)
        {
            var definition = obj as GenericTypeDefinition;
            return definition != null &&
                   base.Equals(obj) &&
                   TypeCount == definition.TypeCount;
        }

        public override int GetHashCode()
        {
            var hashCode = 568399810;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + TypeCount.GetHashCode();
            return hashCode;
        }
    }
    
}
