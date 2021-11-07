using Prototypist.Toolbox;
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.Type;

namespace Tac.SemanticModel
{
    //internal class OverlayTypeReference //: IFrontendType<IVerifiableType>
    //{
    //    public OverlayTypeReference(IFrontendType<IVerifiableType> weakTypeReferance, Overlay overlay)
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
    //                new DelegateBox<IIsPossibly<IFrontendType<IVerifiableType>>>(() => x
    //                    .GetValue()
    //                    .IfIs(y => Possibly.Is(overlay.Convert(y))))));

    //    }

    //    public IIsPossibly<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>> TypeDefinition { get; }

    //    public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
    //    {
    //        return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
    //    }

    //    public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
    //    {
    //        return TypeDefinition.IfIs(x => x.GetValue());
    //    }
    //}

    //internal interface IWeakTypeReference : IConvertableFrontendType<IVerifiableType>, IFrontendCodeElement
    //{
    //    IIsPossibly<IBox<IIsPossibly<IFrontendType<IVerifiableType>>>> TypeDefinition { get; }
    //}


    // now sure why this needs to be a thing
    // can't I just use the type?
    // we have it because sometimes the reference might not find what it is looking for
    internal class WeakTypeReference : IFrontendType<IVerifiableType>
    {
        public WeakTypeReference(IOrType<IFrontendType<IVerifiableType>, IError> typeDefinition)
        {
            TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
        }


        public IOrType<IFrontendType<IVerifiableType>, IError> TypeDefinition { get; }

        //public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        //{
        //    return TypeReferenceStatic.GetBuildIntention(TypeDefinition, context);
        //}


        public IBuildIntention<IVerifiableType> GetBuildIntention(IConversionContext context)
        {
            return TypeDefinition.Is1OrThrow().GetBuildIntention(context);
        }

        //public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        //{
        //    return Possibly.Is(TypeDefinition.GetValue());
        //}

        public IOrType<bool, IError> TheyAreUs(IFrontendType<IVerifiableType> they, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return TypeDefinition.TransformAndFlatten(x=> x.TheyAreUs(they, assumeTrue));
        }

        public IOrType<IOrType<WeakMemberDefinition, IError>, No, IError> TryGetMember(IKey key, List<(IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>)> assumeTrue)
        {
            return TypeDefinition.SwitchReturns(
                    x => x.TryGetMember(key, assumeTrue),
                    x => OrType.Make<IOrType<WeakMemberDefinition, IError>, No, IError>(x)
                );
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetReturn()
        {
            return TypeDefinition.SwitchReturns(
                x => x.TryGetReturn(),
                x => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x)
            );
        }

        public IOrType<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError> TryGetInput()
        {
            return TypeDefinition.SwitchReturns(
                x => x.TryGetInput(),
                x => OrType.Make<IOrType<IFrontendType<IVerifiableType>, IError>, No, IError>(x)
            );
        }

        public IEnumerable<IError> Validate()
        {
            if (TypeDefinition.Is2(out var error)) {
                yield return error;
            }
        }

        public IReadOnlyList<WeakMemberDefinition> GetMembers() => Array.Empty<WeakMemberDefinition>();
    }

    internal static class TypeReferenceStatic
    {
        public static IBuildIntention<IVerifiableType> GetBuildIntention(IBox<IFrontendType<IVerifiableType>> TypeDefinition, IConversionContext context)
        {
            if (TypeDefinition.GetValue() is IFrontendType<IVerifiableType> convertableType)
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
            if (matching is IMatchedTokenMatching matched)
            {
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
                                list.Add(item!);
                                if (w.AllTokens.Any().Not())
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
                    return TokenMatching<IKey>.MakeMatch(tokenMatching, new GenericNameKey(new NameKey(typeName.Item), list.Select(x => OrType.Make<IKey, IError>(x)).ToArray().ToArray()),  genericMatched.EndIndex);
                }
                return TokenMatching<IKey>.MakeMatch(tokenMatching, new NameKey(typeName.Item),matched.EndIndex);
            }

            return TokenMatching<IKey>.MakeNotMatch(matching.Context);
        }
    }

    internal class TypeReferanceMaker : IMaker<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>>
    {
        public ITokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .Has(new TypeNameMaker())
                .ConvertIfMatched(name => new TypeReferancePopulateScope(name), tokenMatching);
        }

    }

    internal class TypeReferancePopulateScope : ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>
    {
        private readonly IKey key;

        public TypeReferancePopulateScope(IKey typeName)
        {
            key = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public ISetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            var type = context.TypeProblem.CreateTypeReference(scope, key, new WeakTypeReferenceConverter());
            return new SetUpResult<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>(new TypeReferanceResolveReference(
                type), OrType.Make<Tpn.TypeProblem2.TypeReference, IError>(type));
        }
    }

    internal class TypeReferanceResolveReference : IResolve<IBox<IFrontendType<IVerifiableType>>>
    {
        private readonly Tpn.TypeProblem2.TypeReference type;

        public TypeReferanceResolveReference(Tpn.TypeProblem2.TypeReference type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IBox<IFrontendType<IVerifiableType>> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            // Is1OrThrow is wrong here! 
            // this has to pass the or type onwards
            return new FuncBox<IFrontendType<IVerifiableType>>(()=> context.GetType(type, stack).Is1OrThrow());
        }
    }

}
