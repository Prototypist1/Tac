﻿using Prototypist.LeftToRight;
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
        bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out IReturnable result);
    }

    public class GenericTypeDefinition : ICodeElement, IGenericTypeDefinition, IReturnable
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

        public bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out IReturnable result)
        {
            if (genericTypeParameters.Select(x => x.Definition).SetEqual(TypeParameterDefinitions).Not())
            {
                result = default;
                return false;
            }

            result = new TypeDefinition(new GenericScope(Scope, genericTypeParameters),Key);
            return true;
        }

        public IReturnable ReturnType(IElementBuilders elementBuilders)
        {
            return this;
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

        internal bool Accepts(IReturnable b)
        {
            // TODO generic constraints
            return true;
        }
    }

    public class GenericTypeParameter
    {
        public GenericTypeParameter(IBox<IReturnable> typeDefinition, GenericTypeParameterDefinition definition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public IBox<IReturnable> TypeDefinition { get; }
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
        private readonly Box<IReturnable> box = new Box<IReturnable>();

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
            var encolsing = context.TryAddType(nameKey, box);

            var resolvable = scope.ToResolvable();
            var nextContext = context.Child(this, scope);
            lines.Select(x => x.Run(nextContext)).ToArray();
            return new GenericTypeDefinitionResolveReferance(nameKey, genericParameters, resolvable, box, make);
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

    }

    public class GenericTypeDefinitionResolveReferance : IResolveReference<GenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly GenericTypeParameterDefinition[] genericParameters;
        private readonly IResolvableScope scope;
        private readonly Box<IReturnable> box;
        private readonly Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make;

        public GenericTypeDefinitionResolveReferance(
            NameKey nameKey, 
            GenericTypeParameterDefinition[] genericParameters, 
            IResolvableScope scope, 
            Box<IReturnable> box, 
            Func<NameKey, IResolvableScope, GenericTypeParameterDefinition[], GenericTypeDefinition> make)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }
        
        public GenericTypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(nameKey, scope, genericParameters));
        }
    }

}
