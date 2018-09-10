using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public interface ITypeDefinition: ICodeElement, IScoped
    {

    }
    
    public class TypeDefinition: ITypeDefinition
    {
        public TypeDefinition(AbstractName key, IScope scope)
        {
            Scope = scope;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AbstractName Key { get; }

        public IScope Scope { get; }

        public override bool Equals(object obj)
        {
            return obj is TypeDefinition definition && definition != null &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key) &&
                   EqualityComparer<IScope>.Default.Equals(Scope, definition.Scope);
        }

        public override int GetHashCode()
        {
            var hashCode = -1628597129;
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<IScope>.Default.GetHashCode(Scope);
            return hashCode;
        }
        
        public ITypeDefinition ReturnType(ScopeStack scope) {
            return RootScope.TypeType;
        }
    }

    public class GenericTypeDefinition : ICodeElement
    {
        public GenericTypeDefinition(AbstractName key, ObjectScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public AbstractName Key { get; }

        public ObjectScope Scope { get; }

        public GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out TypeDefinition result) {
            if (genericTypeParameters.Select(x => x.Definition).SetEqual(TypeParameterDefinitions).Not()) {
                result = default;
                return false;
            }

            result = new TypeDefinition(Key, new GenericScope(Scope, genericTypeParameters));
            return true;
        }

        public ITypeDefinition ReturnType(ScopeStack scope) => RootScope.TypeType;
    }

    public class GenericTypeParameterDefinition
    {
        public GenericTypeParameterDefinition(AbstractName name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public AbstractName Name { get; }

        internal bool Accepts(TypeDefinition b) {
            // TODO generic constraints
            return true;
        }
    }

    public class GenericTypeParameter {
        public GenericTypeParameter(TypeDefinition typeDefinition , GenericTypeParameterDefinition definition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public TypeDefinition TypeDefinition { get; }
        public GenericTypeParameterDefinition Definition { get; }
    }
    
}
