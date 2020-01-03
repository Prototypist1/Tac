using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model
{
    //internal class OverlayTypeReference //: IFrontendType
    //{
    //    public OverlayTypeReference(IFrontendType weakTypeReferance, Overlay overlay)
    //    {
    //        if (weakTypeReferance == null)
    //        {
    //            throw new ArgumentNullException(nameof(weakTypeReferance));
    //        }

    //        if (overlay == null)
    //        {
    //            throw new ArgumentNullException(nameof(overlay));
    //        }

    //        TypeDefinition = weakTypeReferance.TypeDefinition.IfIs(x =>
    //            Possibly.Is(
    //                new DelegateBox<IIsPossibly<IFrontendType>>(() => x
    //                    .GetValue()
    //                    .IfIs(y => Possibly.Is(overlay.Convert(y))))));

    //    }

    //    public IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }

    //    public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
    //    {
    //        return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
    //    }

    //    public IIsPossibly<IFrontendType> Returns()
    //    {
    //        return TypeDefinition.IfIs(x => x.GetValue());
    //    }
    //}

    //internal interface IWeakTypeReference : IConvertableFrontendType<IVerifiableType>, IFrontendCodeElement
    //{
    //    IIsPossibly<IBox<IIsPossibly<IFrontendType>>> TypeDefinition { get; }
    //}


    // now sure why this needs to be a thing
    // can't I just use the type?
    //internal class WeakTypeReference //: IFrontendType
    //{
    //    public WeakTypeReference(IBox<IFrontendType> typeDefinition)
    //    {
    //        TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
    //    }

    //    public IBox<IFrontendType> TypeDefinition { get; }

    //    public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
    //    {
    //        return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
    //    }

    //    public IIsPossibly<IFrontendType> Returns()
    //    {
    //        return Possibly.Is(TypeDefinition.GetValue());
    //    }
    //}

    internal static class TypeReferenceStatic
    {
        public static IBuildIntention<IVerifiableType> GetBuildIntention(IBox<IFrontendType> TypeDefinition, IConversionContext context)
        {
            if (TypeDefinition.GetValue() is IConvertableFrontendType<IVerifiableType> convertableType)
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

    internal class TypeReferanceMaker : IMaker<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new TypeNameMaker(), out var name);

            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new TypeReferancePopulateScope(name));
            }

            return TokenMatching<ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>>.MakeNotMatch(matching.Context);
        }


        public class TypeReferancePopulateScope : ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference>
        {
            private readonly IKey key;

            public TypeReferancePopulateScope(IKey typeName)
            {
                key = typeName ?? throw new ArgumentNullException(nameof(typeName));
            }

            public ISetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var type = context.TypeProblem.CreateTypeReference(scope,key, new WeakTypeReferenceConverter());
                return new SetUpResult<IFrontendType, Tpn.TypeProblem2.TypeReference>(new TypeReferanceResolveReference(
                    type), type);
            }
        }

        public class TypeReferanceResolveReference : IResolve<IFrontendType>
        {
            private readonly Tpn.TypeProblem2.TypeReference type;

            public TypeReferanceResolveReference(Tpn.TypeProblem2.TypeReference type)
            {
                this.type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IBox<IFrontendType> Run(Tpn.ITypeSolution context)
            {
                return context.GetTypeReference(type);
            }
        }
    }

}
