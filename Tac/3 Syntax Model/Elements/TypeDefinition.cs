using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public interface ITypeDefinition : ICodeElement, IScoped
    {

    }

    public class TypeDefinition : ITypeDefinition
    {
        public TypeDefinition(IScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IScope Scope { get; }
        
        public IBox<ITypeDefinition> ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.TypeType);
        }
    }

    public class TypeDefinitionMaker : IMaker<TypeDefinition>
    {
        public TypeDefinitionMaker(Func<IScope, TypeDefinition> make)
        {
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        private Func<IScope, TypeDefinition> Make { get; }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out Steps.PopulateScope<TypeDefinition> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                            .Has(ElementMatcher.KeyWord("type"), out var _)
                            .OptionalHas(ElementMatcher.IsName, out AtomicToken typeName)
                            .Has(ElementMatcher.IsBody, out CurleyBacketToken body)
                            .IsMatch)
            {
                var scope = new ObjectScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);


                result = PopulateScope(scope, elements, typeName);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<TypeDefinition> PopulateScope(ObjectScope scope, Steps.PopulateScope<ICodeElement>[] elements,AtomicToken typeName)
        {
            return (tree) =>
            {
                elements.Select(x => x(tree)).ToArray();
                var box = new Box<TypeDefinition>();
                if (typeName != null)
                {
                    var encolsing = tree.Scopes(scope).Skip(1).First();
                    encolsing.Cast<StaticScope>().TryAddStaticType(new NameKey(typeName.Item),box);
                }
                return DetermineInferedTypes(scope,box);
            };
        }

        private Steps.DetermineInferedTypes<TypeDefinition> DetermineInferedTypes(ObjectScope scope, Box<TypeDefinition> box)
        {
            return () => ResolveReferance(scope, box);
        }

        private Steps.ResolveReferance<TypeDefinition> ResolveReferance(ObjectScope scope, Box<TypeDefinition> box)
        {
            return (tree) =>
            {
                return box.Fill(Make(scope));
            };
        }
    }

    public class NamedTypeDefinition : TypeDefinition
    {
        public NamedTypeDefinition(IKey key, IScope scope) : base(scope)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IKey Key { get; }
        
    }

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

        public NameKey Key { get {
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
