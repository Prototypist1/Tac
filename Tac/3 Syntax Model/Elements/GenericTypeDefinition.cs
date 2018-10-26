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
    public interface IGenericTypeDefinition
    {
        IFinalizedScope Scope { get; }
        GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }
        // why does this know it's own key?
        IKey Key { get; }
    }

    public class WeakGenericTypeDefinition : IWeakCodeElement, IWeakReturnable
    {
        public delegate WeakGenericTypeDefinition Make(NameKey key, IWeakFinalizedScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions);

        public WeakGenericTypeDefinition(NameKey key, IWeakFinalizedScope scope, GenericTypeParameterDefinition[] typeParameterDefinitions)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            TypeParameterDefinitions = typeParameterDefinitions ?? throw new ArgumentNullException(nameof(typeParameterDefinitions));
        }

        public IKey Key { get; }

        public IWeakFinalizedScope Scope { get; }

        public GenericTypeParameterDefinition[] TypeParameterDefinitions { get; }

        // huh? this seems to have no uses
        // and that means GenericScope has no uses
        // I have not build that part out yet so it is ok.
        //public bool TryCreateConcrete(IEnumerable<GenericTypeParameter> genericTypeParameters, out IReturnable result)
        //{
        //    if (genericTypeParameters.Select(x => x.Definition).SetEqual(TypeParameterDefinitions).Not())
        //    {
        //        result = default;
        //        return false;
        //    }

        //    result = new TypeDefinition(new GenericScope(Scope, genericTypeParameters),Key);
        //    return true;
        //}

        public IWeakReturnable Returns(IElementBuilders elementBuilders)
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

        internal bool Accepts(IWeakReturnable b)
        {
            // TODO generic constraints
            return true;
        }
    }

    public class GenericTypeParameter
    {
        public GenericTypeParameter(IBox<IWeakReturnable> typeDefinition, GenericTypeParameterDefinition definition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public IBox<IWeakReturnable> TypeDefinition { get; }
        public GenericTypeParameterDefinition Definition { get; }
    }

    public class GenericTypeDefinitionMaker : IMaker<WeakGenericTypeDefinition>
    {
        private readonly WeakGenericTypeDefinition.Make make;

        public GenericTypeDefinitionMaker(WeakGenericTypeDefinition.Make make)
        {
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResult<IPopulateScope<WeakGenericTypeDefinition>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.KeyWord("type"), out var _)
                .Has(ElementMatcher.DefineGenericN, out AtomicToken[] genericTypes)
                .Has(ElementMatcher.IsName, out AtomicToken typeName)
                .Has(ElementMatcher.IsBody, out CurleyBracketToken body)
                .IsMatch)
            {
                return ResultExtension.Good(new GenericTypeDefinitionPopulateScope(
                    new NameKey(typeName.Item),
                    matchingContext.ParseBlock(body),
                    genericTypes.Select(x => new GenericTypeParameterDefinition(x.Item)).ToArray(), 
                    make));
            }

            return ResultExtension.Bad<IPopulateScope<WeakGenericTypeDefinition>>();
        }



    }

    public class GenericTypeDefinitionPopulateScope : IPopulateScope<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly IEnumerable<IPopulateScope<IWeakCodeElement>> lines;
        private readonly GenericTypeParameterDefinition[] genericParameters;
        private readonly WeakGenericTypeDefinition.Make make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public GenericTypeDefinitionPopulateScope(
            NameKey nameKey, 
            IEnumerable<IPopulateScope<IWeakCodeElement>> lines,
            GenericTypeParameterDefinition[] genericParameters, 
            WeakGenericTypeDefinition.Make make)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<WeakGenericTypeDefinition> Run(IPopulateScopeContext context)
        {
            var encolsing = context.Scope.TryAddType(nameKey, box);
            
            var nextContext = context.Child();
            lines.Select(x => x.Run(nextContext)).ToArray();
            return new GenericTypeDefinitionResolveReferance(nameKey, genericParameters, nextContext.GetResolvableScope(), box, make);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

    }

    public class GenericTypeDefinitionResolveReferance : IResolveReference<WeakGenericTypeDefinition>
    {
        private readonly NameKey nameKey;
        private readonly GenericTypeParameterDefinition[] genericParameters;
        private readonly IResolvableScope scope;
        private readonly Box<IWeakReturnable> box;
        private readonly WeakGenericTypeDefinition.Make make;

        public GenericTypeDefinitionResolveReferance(
            NameKey nameKey, 
            GenericTypeParameterDefinition[] genericParameters, 
            IResolvableScope scope, 
            Box<IWeakReturnable> box,
            WeakGenericTypeDefinition.Make make)
        {
            this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }
        
        public WeakGenericTypeDefinition Run(IResolveReferanceContext context)
        {
            return box.Fill(make(nameKey, scope.GetFinalized(), genericParameters));
        }
    }

}
