﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model
{

    internal class OverlayGenericTypeDefinition: IWeakGenericTypeDefinition
    {
        private readonly IWeakGenericTypeDefinition backing;

        public OverlayGenericTypeDefinition(IWeakGenericTypeDefinition backing, Overlay overlay)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            Scope = new OverlayedScope(backing.Scope, overlay);
        }
        
        public IResolvableScope Scope { get; }

        public IIsPossibly<IKey> Key => backing.Key;
        public IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions=> backing.TypeParameterDefinitions;
        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context) => backing.Cast<IFrontendCodeElement<IGenericInterfaceDefinition>>().GetBuildIntention(context);
        public OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters) => backing.Overlay(typeParameters);
        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(ConversionContext context) => backing.Cast<IFrontendType<IGenericType>>().GetBuildIntention(context); 
        IIsPossibly<IFrontendType<IVerifiableType>> IFrontendCodeElement<IGenericInterfaceDefinition>.Returns() => Possibly.Is(this);
    }

    internal class ExternalGenericType : IWeakGenericTypeDefinition
    {
        private readonly IGenericInterfaceDefinition type;

        public ExternalGenericType(GenericNameKey key,IGenericInterfaceDefinition type)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            

            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.TypeParameterDefinitions = type.TypeParameterKeys.Select(x => Possibly.Is( new _3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder(x))).ToArray();
            this.Key = Possibly.Is(key);
            this.Scope = new ExteranlResolvableScope(type.Scope);
        }

        public IIsPossibly<IKey> Key {get;}

        public IResolvableScope Scope {get;}

        public IIsPossibly<_3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        
        public OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            var overlay = new Overlay(typeParameters.ToDictionary(x => x.parameterDefinition, x => x.frontendType));
            if (typeParameters.All(x => !(x.frontendType is Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new OverlayTypeDefinition(
                    new WeakTypeDefinition(Scope, Key), overlay));
            }
            else
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new OverlayGenericTypeDefinition(
                    this, overlay).Cast<IFrontendGenericType>());
            }
        }

        public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is(this);
        }

        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context)
        {
            return new BuildIntention<IGenericInterfaceDefinition>(type, () => { });
        }

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(ConversionContext context) => GetBuildIntention(context);
    }

    internal interface IWeakGenericTypeDefinition: IFrontendCodeElement<IGenericInterfaceDefinition>, IScoped, IFrontendType<IGenericType>, IFrontendGenericType
    {
        IIsPossibly<IKey> Key { get; }
    }

    internal class WeakGenericTypeDefinition : IWeakGenericTypeDefinition
    {
        public WeakGenericTypeDefinition(
            IIsPossibly<NameKey> key,
            IResolvableScope scope,
            IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions)
        {
            this.TypeParameterDefinitions = TypeParameterDefinitions ?? throw new ArgumentNullException(nameof(TypeParameterDefinitions));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IIsPossibly<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        public IIsPossibly<IKey> Key { get; }
        public IResolvableScope Scope { get; }

        public IBuildIntention<IGenericInterfaceDefinition> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = GenericInterfaceDefinition.Create();
            return new BuildIntention<IGenericInterfaceDefinition>(toBuild, () =>
            {
                maker.Build(
                    Scope.Convert(context),
                    TypeParameterDefinitions.Select(x=>x.GetOrThrow().Convert(context)).ToArray());
            });
        }

        public OrType<IFrontendGenericType, IFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters)
        {
            // I kept reusing this code..
            var overlay =  new Overlay(typeParameters.ToDictionary(x=>x.parameterDefinition,x=>x.frontendType));
            if (typeParameters.All(x => !(x.frontendType is Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder)))
            {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new OverlayTypeDefinition(
                    new WeakTypeDefinition(Scope,Key),overlay));
            }
            else {
                return new OrType<IFrontendGenericType, IFrontendType<IVerifiableType>>(new OverlayGenericTypeDefinition(
                    this, overlay).Cast<IFrontendGenericType>());
            }
        }

        IBuildIntention<IGenericType> IConvertable<IGenericType>.GetBuildIntention(ConversionContext context) => GetBuildIntention(context);

        IIsPossibly<IFrontendType<IVerifiableType>> IFrontendCodeElement<IGenericInterfaceDefinition>.Returns()
        {
            return Possibly.Is(this);
        }
    }
    
    internal class GenericTypeDefinitionMaker : IMaker<IPopulateScope<WeakGenericTypeDefinition>>
    {

        public GenericTypeDefinitionMaker()
        {
        }

        public ITokenMatching<IPopulateScope<WeakGenericTypeDefinition>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("type"), out var _)
                .Has(new DefineGenericNMaker(), out var genericTypes)
                .Has(new NameMaker(), out var typeName)
                .Has(new BodyMaker(), out var body);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new GenericTypeDefinitionPopulateScope(
                        new NameKey(typeName.Item),
                        tokenMatching.Context.ParseBlock(body),
                        genericTypes.Select(x => 
                        new Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder(new NameKey(x))).ToArray()));
            }

            return TokenMatching<IPopulateScope<WeakGenericTypeDefinition>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<WeakGenericTypeDefinition> PopulateScope(
                NameKey nameKey,
                IEnumerable<IPopulateScope<IFrontendCodeElement<ICodeElement>>> lines,
                Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters)
        {
            return new GenericTypeDefinitionPopulateScope(
                nameKey,
                lines,
                genericParameters);
        }
        public static IPopulateBoxes<WeakGenericTypeDefinition> PopulateBoxes(NameKey nameKey,
                IResolvableScope scope,
                Box<IIsPossibly<IFrontendGenericType>> box,
                Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters,
                IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] lines)
        {
            return new GenericTypeDefinitionResolveReferance(nameKey,
                scope,
                box,
                genericParameters,
                lines);
        }

        private class GenericTypeDefinitionPopulateScope : IPopulateScope<WeakGenericTypeDefinition>
        {
            private readonly NameKey nameKey;
            private readonly IEnumerable<IPopulateScope<IFrontendCodeElement<ICodeElement>>> lines;
            private readonly Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters;
            private readonly Box<IIsPossibly<IFrontendGenericType>> box = new Box<IIsPossibly<IFrontendGenericType>>();

            public GenericTypeDefinitionPopulateScope(
                NameKey nameKey,
                IEnumerable<IPopulateScope<IFrontendCodeElement<ICodeElement>>> lines,
                Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
                this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
                this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
            }

            public IPopulateBoxes<WeakGenericTypeDefinition> Run(IPopulateScopeContext context)
            {
                var encolsing = context.Scope.TryAddGeneric(nameKey, box);

                var nextContext = context.TemplateChild(genericParameters);
                var nextLines = lines.Select(x => x.Run(nextContext)).ToArray();
                return new GenericTypeDefinitionResolveReferance(nameKey, nextContext.GetResolvableScope(), box, genericParameters, nextLines);
            }

            public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }
        }

        private class GenericTypeDefinitionResolveReferance : IPopulateBoxes<WeakGenericTypeDefinition>
        {
            private readonly NameKey nameKey;
            private readonly IResolvableScope scope;
            private readonly Box<IIsPossibly<IFrontendGenericType>> box;
            private readonly Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters;
            private readonly IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] lines;

            public GenericTypeDefinitionResolveReferance(
                NameKey nameKey,
                IResolvableScope scope,
                Box<IIsPossibly<IFrontendGenericType>> box,
                Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] genericParameters,
                IPopulateBoxes<IFrontendCodeElement<ICodeElement>>[] lines)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.genericParameters = genericParameters ?? throw new ArgumentNullException(nameof(genericParameters));
                this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
            }

            public IIsPossibly<WeakGenericTypeDefinition> Run(IResolveReferenceContext context)
            {
                // hmm getting the template down here is hard
                // scope mostly comes from context
                // why is that?

                var nextLines = lines.Select(x => x.Run(context)).ToArray();
                return box.Fill(Possibly.Is(new WeakGenericTypeDefinition(
                    Possibly.Is(nameKey),
                    scope,
                    genericParameters.Select(x => Possibly.Is(x)).ToArray())));
            }
        }


    }
}