﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model
{
    // TODO 
    // TODO
    // TOOD
    // YOU ARE HERE
    // YOU ARE HERE 
    // huh.
    // I need to think about this.
    // what is overlayTypeReference ??
    // it is a generic type reference
    // but I should actually have a generic type reference
    // and that should be the thing you can overlay 
    // and only the concrete type reference should be convertable 

    internal class OverlayTypeReference : IWeakTypeReference
    {
        public OverlayTypeReference(IWeakTypeReference weakTypeReferance, Overlay overlay)
        {
            if (weakTypeReferance == null)
            {
                throw new ArgumentNullException(nameof(weakTypeReferance));
            }
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));

            TypeDefinition = weakTypeReferance.TypeDefinition.IfIs(x =>
                Possibly.Is(
                    new DelegateBox<IIsPossibly<IFrontendType>>(() => x
                        .GetValue()
                        .IfIs(y => Possibly.Is(overlay.Convert(y))))));

        }

        private readonly Overlay overlay;
        public IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get;}

        // TODO this code is dup
        // should it be shared?


        public IBuildIntention<ITypeReferance> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = TypeReference.Create();
            return new BuildIntention<ITypeReferance>(toBuild, () =>
            {
                maker.Build(TypeDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context));
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return TypeDefinition.IfIs(x => x.GetValue());
        }
    }

    internal interface IWeakTypeReference : IConvertable<ITypeReferance>, IFrontendType {
        IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }
    }

    //internal class ExternalTypeDefinition : IFrontendType<IVerifiableType>
    //{
    //    private readonly IVerifiableType type;

    //    public ExternalTypeDefinition(IVerifiableType type)
    //    {
    //        this.type = type ?? throw new ArgumentNullException(nameof(type));
    //    }

    //    public IBuildIntention<IVerifiableType> GetBuildIntention(TransformerExtensions.ConversionContext context)
    //    {
    //        return new BuildIntention<IVerifiableType>(type, () => {});
    //    }
    //}

    //internal class ExternalTypeReference : IWeakTypeReferance
    //{
    //    public ExternalTypeReference(IFrontendType<IVerifiableType> type)
    //    {
    //        this.type = type ?? throw new ArgumentNullException(nameof(type));
    //        TypeDefinition = Possibly.Is(new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(type)));
    //    }

    //    private readonly IFrontendType<IVerifiableType> type;

    //    public IIsPossibly<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>> TypeDefinition { get; }

    //    public IBuildIntention<ITypeReferance> GetBuildIntention(TransformerExtensions.ConversionContext context)
    //    {
    //        var (toBuild, maker) = TypeReference.Create();
    //        return new BuildIntention<ITypeReferance>(toBuild, () =>
    //        {
    //            maker.Build(type.Convert(context));
    //        });
    //    }

    //    public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
    //    {
    //        return TypeDefinition.IfIs(x => x.GetValue());
    //    }

    //    IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    //}

    internal class WeakTypeReference : IWeakTypeReference
    {
        public WeakTypeReference(IIsPossibly<IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>> typeDefinition)
        {
            ConvertableTypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }

        public IIsPossibly<IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>> ConvertableTypeDefinition { get; }

        public IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition => ConvertableTypeDefinition;

        public IBuildIntention<ITypeReferance> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = TypeReference.Create();
            return new BuildIntention<ITypeReferance>(toBuild, () =>
            {
                maker.Build(ConvertableTypeDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context));
            });
        }
        
        public IIsPossibly<IConvertableFrontendType<IVerifiableType>> Returns()
        {
            return ConvertableTypeDefinition.IfIs(x => x.GetValue());
        }
    }

    internal class KeyMatcher : IMaker<IKey>
    {
        public ITokenMatching<IKey> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
                .Has(new NameMaker(), out var typeName);
            
            var list = new List<IKey>();
            var genericMatachig = matching
                .HasSquare(x => {
                    while (true)
                    {
                        // colin, why! w x y z
                        // you are an adult arn'y you?
                        var item = default(IKey);
                        var y = x.HasLine(z => z.Has(new KeyMatcher(), out item));
                        if (y is IMatchedTokenMatching w)
                        {
                            x = w;
                            list.Add(item);
                            if (w.Tokens.Any().Not()) {
                                return w;
                            }
                        }
                        else
                        {
                            return y;
                        }
                    }
                });

            if (genericMatachig is IMatchedTokenMatching genericMatched)
            {
                return TokenMatching<IKey>.MakeMatch(genericMatched.Tokens,genericMatched.Context, new GenericNameKey(new NameKey(typeName.Item),list.ToArray()));
            }

            if (matching is IMatchedTokenMatching matched) {
                return TokenMatching<IKey>.MakeMatch(matched.Tokens, matched.Context, new NameKey(typeName.Item));
            }

            return TokenMatching<IKey>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferanceMaker : IMaker<IPopulateScope<WeakTypeReference>>
    {
        public ITokenMatching<IPopulateScope<WeakTypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new TypeMaker(), out var typeReferancePopulateScope);
            
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakTypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    typeReferancePopulateScope);
            }

            return TokenMatching<IPopulateScope<WeakTypeReference>>.MakeNotMatch(matching.Context);
        }


        public static IPopulateScope<WeakTypeReference> PopulateScope(IKey typeName)
        {
            return new TypeReferancePopulateScope(typeName);
        }
        public static IPopulateBoxes<WeakTypeReference> PopulateBoxes(IResolvableScope scope, Box<IIsPossibly<WeakTypeReference>> box, IKey key)
        {
            return new TypeReferanceResolveReference(scope, box, key);
        }


        private class TypeReferancePopulateScope : IPopulateScope<WeakTypeReference>
        {
            private readonly IKey key;
            private readonly Box<IIsPossibly<WeakTypeReference>> box = new Box<IIsPossibly<WeakTypeReference>>();

            public TypeReferancePopulateScope(IKey typeName)
            {
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IPopulateBoxes<WeakTypeReference> Run(IPopulateScopeContext context)
            {
                return new TypeReferanceResolveReference(
                    context.GetResolvableScope(),
                    box,
                    key);
            }
        }

        private class TypeReferanceResolveReference : IPopulateBoxes<WeakTypeReference>
        {
            private readonly IResolvableScope scope;
            private readonly Box<IIsPossibly<WeakTypeReference>> box;
            private readonly IKey key;

            public TypeReferanceResolveReference(IResolvableScope scope, Box<IIsPossibly<WeakTypeReference>> box, IKey key)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<WeakTypeReference> Run(IResolveReferenceContext context)
            {
                return box.Fill(Possibly.Is(new WeakTypeReference(scope.PossiblyGetType(key))));
            }
        }
    }

}
