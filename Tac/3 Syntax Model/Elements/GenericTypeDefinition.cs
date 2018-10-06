using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IGenericTypeDefinition{
        GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
        bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out ITypeDefinition result);
    }

    public class GenericTypeDefinition : ICodeElement, ITypeDefinition, IGenericTypeDefinition
    {
        public GenericTypeDefinition(NameKey key, IResolvableScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public IKey Key { get; }

        public IResolvableScope Scope { get; }

        public GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        public bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out ITypeDefinition result)
        {
            if (genericTypeParameters.Select(x => x.Definition).SetEqual(TypeParameterDefinitions).Not())
            {
                result = default;
                return false;
            }

            result = new TypeDefinition(new GenericScope(Scope, genericTypeParameters),Key);
            return true;
        }

        public IBox<ITypeDefinition> ReturnType()
        {
            return root.GetTypeOrThrow(RootScope.TypeType);
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
        public GenericTypeParameter(IBox<ITypeDefinition> typeDefinition, GenericTypeParameterDefinition definition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public IBox<ITypeDefinition> TypeDefinition { get; }
        public GenericTypeParameterDefinition Definition { get; }
    }

    public class GenericTypeDefinitionMaker : IMaker<GenericTypeDefinition>
    {
        private readonly Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make;

        public GenericTypeDefinitionMaker(Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make)
        {
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResult<IPopulateScope<GenericTypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .Has(ElementMatcher.DefineGenericN, out AtomicToken[] genericTypes)
                .Has(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .IsMatch)
            {

                var scope = Scope.LocalStaticScope();

                var elementMatchingContext = matchingContext.Child(scope);
                var elements = elementMatchingContext.ParseBlock(body);

                var genericParameters = genericTypes.Select(x => new GenericTypeParameterDefinition(x.Item)).ToArray();

                return ResultExtension.Good(new GenericTypeDefinitionPopulateScope(new NameKey(typeName.Item), elements, scope, genericParameters, make));
            }

            return ResultExtension.Bad<IPopulateScope<GenericTypeDefinition>>();
        }



    }

    public class GenericTypeDefinitionPopulateScope : IPopulateScope<GenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly ILocalStaticScope scope;
        private readonly IEnumerable<IPopulateScope<ICodeElement>> lines;
        private readonly GenericTypeParameterDefinition[] genericParameters;
        private readonly Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make;

        public GenericTypeDefinitionPopulateScope(NameKey nameKey, IEnumerable<IPopulateScope<ICodeElement>> lines, ILocalStaticScope scope, GenericTypeParameterDefinition[] genericParameters, Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<GenericTypeDefinition> Run(IPopulateScopeContext context)
        {
            var box = new Box<ITypeDefinition>();

            var encolsing = context.TryAddType(nameKey, box);

            var resolvable = scope.ToResolvable();
            var nextContext = context.Child(this, resolvable);
            lines.Select(x => x.Run(nextContext)).ToArray();
            return new GenericTypeDefinitionResolveReferance(nameKey, genericParameters, resolvable, box, make);
        }

    }

    public class GenericTypeDefinitionResolveReferance : IResolveReference<GenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly GenericTypeParameterDefinition[] genericParameters;
        private readonly IResolvableScope scope;
        private readonly Box<ITypeDefinition> box;
        private readonly Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make;

        public GenericTypeDefinitionResolveReferance(NameKey nameKey, GenericTypeParameterDefinition[] genericParameters, IResolvableScope scope, Box<ITypeDefinition> box, Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.genericParameters = genericParameters;
            this.scope = scope;
            this.box = box;
            this.make = make;
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return context.Tree.root.GetTypeOrThrow(RootScope.TypeType);
        }

        public GenericTypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(nameKey, scope, genericParameters));
        }
    }

}
