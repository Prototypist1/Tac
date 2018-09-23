using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class GenericTypeDefinition : ICodeElement, ITypeDefinition
    {
        public GenericTypeDefinition(NameKey key, ObjectScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public IKey Key { get; }

        public ObjectScope Scope { get; }

        public GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out TypeDefinition result)
        {
            if (genericTypeParameters.Select(x => x.Definition).SetEqual(TypeParameterDefinitions).Not())
            {
                result = default;
                return false;
            }

            result = new TypeDefinition(new GenericScope(Scope, genericTypeParameters));
            return true;
        }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.TypeType);
        }
    }


    public class GenericTypeParameterDefinition
    {
        public GenericTypeParameterDefinition(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public NameKey Key
        {
            get
            {
                return new NameKey(Name);
            }
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericTypeParameterDefinition definition &&
                   Name == definition.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        internal bool Accepts(ITypeDefinition b)
        {
            // TODO generic constraints
            return true;
        }
    }

    public class GenericTypeParameter
    {
        public GenericTypeParameter(ITypeDefinition typeDefinition, GenericTypeParameterDefinition definition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public ITypeDefinition TypeDefinition { get; }
        public GenericTypeParameterDefinition Definition { get; }
    }

}
