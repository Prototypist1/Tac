﻿using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model
{
    internal class OverlayTypeReference : IWeakTypeReference
    {
        public OverlayTypeReference(IWeakTypeReference weakTypeReferance, Overlay overlay)
        {
            if (weakTypeReferance == null)
            {
                throw new ArgumentNullException(nameof(weakTypeReferance));
            }

            if (overlay == null)
            {
                throw new ArgumentNullException(nameof(overlay));
            }

            TypeDefinition = weakTypeReferance.TypeDefinition.IfIs(x =>
                Possibly.Is(
                    new DelegateBox<IIsPossibly<IFrontendType>>(() => x
                        .GetValue()
                        .IfIs(y => Possibly.Is(overlay.Convert(y))))));

        }

        public IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return TypeDefinition.IfIs(x => x.GetValue());
        }
    }

    internal interface IWeakTypeReference : IConvertableFrontendType<IVerifiableType>, IFrontendCodeElement
    {
        IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }
    }

    internal class WeakTypeReference : IWeakTypeReference
    {
        public WeakTypeReference(IIsPossibly<IBox<IIsPossibly<IFrontendType>>> typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }

        public IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }

        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return TypeDefinition.IfIs(x => x.GetValue());
        }
    }

    internal static class TypeReferenceStatic
    {
        public static IBuildIntention<IVerifiableType> GetBuildIntention(IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition, IConversionContext context)
        {
            if (TypeDefinition.GetOrThrow().GetValue().GetOrThrow() is IConvertableFrontendType<IVerifiableType> convertableType)
            {
                return convertableType.GetBuildIntention(context);
            }
            else
            {
                throw new Exception("can not be built, type is not convertable");
            }
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
                .HasSquare(x =>
                {
                    while (true)
                    {
                        // colin, why! w x y z
                        // you are an adult arn't you?
                        var item = default(IKey);
                        var y = x.HasLine(z => z.Has(new KeyMatcher(), out item));
                        if (y is IMatchedTokenMatching w)
                        {
                            x = w;
                            list.Add(item);
                            if (w.Tokens.Any().Not())
                            {
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
                return TokenMatching<IKey>.MakeMatch(genericMatched.Tokens, genericMatched.Context, new GenericNameKey(new NameKey(typeName.Item), list.ToArray()));
            }

            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IKey>.MakeMatch(matched.Tokens, matched.Context, new NameKey(typeName.Item));
            }

            return TokenMatching<IKey>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferanceMaker : IMaker<ISetUp<IWeakTypeReference, Tpn.ITypeReference>>
    {
        public ITokenMatching<ISetUp<IWeakTypeReference, Tpn.ITypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new TypeNameMaker(), out var name);

            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<IWeakTypeReference, Tpn.ITypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new TypeReferancePopulateScope(name));
            }

            return TokenMatching<ISetUp<IWeakTypeReference, Tpn.ITypeReference>>.MakeNotMatch(matching.Context);
        }


        public static ISetUp<WeakTypeReference, Tpn.ITypeReference> PopulateScope(IKey typeName)
        {
            return new TypeReferancePopulateScope(typeName);
        }
        public static IResolve<WeakTypeReference> PopulateBoxes(IKey key)
        {
            return new TypeReferanceResolveReference(key);
        }


        private class TypeReferancePopulateScope : ISetUp<WeakTypeReference, Tpn.ITypeReference>
        {
            private readonly IKey key;

            public TypeReferancePopulateScope(IKey typeName)
            {
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public ISetUpResult<WeakTypeReference, Tpn.ITypeReference> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var type = context.TypeProblem.CreateTypeReference(scope,key);
                return new SetUpResult<WeakTypeReference, Tpn.ITypeReference>(new TypeReferanceResolveReference(
                    key), type);
            }
        }

        private class TypeReferanceResolveReference : IResolve<WeakTypeReference>
        {
            private readonly IKey key;

            public TypeReferanceResolveReference( IKey key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public IIsPossibly<WeakTypeReference> Run(IResolvableScope scope, IResolveContext context)
            {
                return Possibly.Is(new WeakTypeReference(scope.PossiblyGetType(key)));
            }
        }
    }

}
