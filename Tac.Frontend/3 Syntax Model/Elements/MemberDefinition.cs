using Prototypist.Toolbox;
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
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMemberDefinitionMaker = AddElementMakers(
            () => new MemberDefinitionMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MemberDefinitionMaker = StaticMemberDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel
{

    // very tac-ian 
    internal static class MemberDefinitionShared
    {

        public static IMemberDefinition Convert(IBox<IOrType<IFrontendType, IError>> Type, IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (def, builder) = MemberDefinition.Create();

            //uhh seems bad
            var buildIntention = Type.GetValue().TransformInner(x => x.CastTo<IConvertable<IVerifiableType>>().GetBuildIntention(context));
            var built = buildIntention.TransformInner(x => { x.Build(); return x.Tobuild; });
            builder.Build(Key, built.Is1OrThrow(), ReadOnly);
            return def;
        }
        public static IBuildIntention<IMemberDefinition> GetBuildIntention(IBox<IOrType<IFrontendType, IError>> Type, IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    Type.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
                    ReadOnly);
            });
        }

    }

    // is this really a frontend type??
    // do I really need an interface?
    // an internal interface?
    //internal interface IWeakMemberDefinition:  IConvertable<IMemberDefinition>
    //{
    //    IOrType<IBox<IFrontendType>, IError> Type { get; }
    //    bool ReadOnly { get; }
    //    IKey Key { get; }
    //    IMemberDefinition Convert(IConversionContext context);
    //}

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    internal class WeakMemberDefinition : IConvertable<IMemberDefinition>, IValidate, IReturn
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IBox<IOrType<IFrontendType, IError>> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IOrType<IFrontendType, IError>> Type { get; }
        public bool ReadOnly { get; }
        public IKey Key { get; }

        public IMemberDefinition Convert(IConversionContext context)
        {
            return MemberDefinitionShared.Convert(Type, context, ReadOnly, Key);
        }

        public IBuildIntention<IMemberDefinition> GetBuildIntention(IConversionContext context)
        {
            return MemberDefinitionShared.GetBuildIntention(Type, context, ReadOnly, Key);
        }

        public IOrType<IFrontendType, IError> Returns()
        {
            return OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.RefType(Type.GetValue().TransformInner(x => x)));
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in Type.GetValue().SwitchReturns(x => x.Validate(), x => new[] { x }))
            {
                yield return error;
            }
        }
    }

    // these two share a lot of code
    // but is all boiler plate so I don't care
    // {0CFF70E2-9691-4A79-9327-11385BFA3DC9}
    internal class MemberDefinitionMaker : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    {
        public MemberDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMaker())
                .Has(new NameMaker())
                .ConvertIfMatched((type, nameToken) => new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type));
        }

        //public static ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> PopulateScope(
        //    IKey item, 
        //    bool v, 
        //    ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> typeToken)
        //{
        //    return new MemberDefinitionPopulateScope(item, v,  typeToken);
        //}


    }

    internal class MemberDefinitionPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
    {
        private readonly IKey memberName;
        private readonly bool isReadonly;
        private readonly ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> type;

        public MemberDefinitionPopulateScope(IKey item, bool v, ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference> typeToken)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
        }

        public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            var type = this.type.Run(scope, context.CreateChild(this));


            var member = context.TypeProblem.CreateMember(scope, memberName, type.SetUpSideNode.TransformInner(x => x.Key()), new WeakMemberDefinitionConverter(isReadonly, memberName));


            return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new MemberDefinitionResolveReferance(
                member), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
        }

    }

    internal class MemberDefinitionResolveReferance : IResolve<IBox<WeakMemberReference>>
    {
        private readonly Tpn.TypeProblem2.Member member;

        public MemberDefinitionResolveReferance(Tpn.TypeProblem2.Member member)
        {
            this.member = member ?? throw new ArgumentNullException(nameof(member));
        }

        public IBox<WeakMemberReference> Run(Tpn.TypeSolution context)
        {
            return new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(member)));
        }
    }

    // you are here
    // these two share a lot of code
    // you should combine them in to one class that is context awake
    // if it is in an assignment that is the root of a line in an object/module it does one thing
    // if it is the root of a line in a type it does anotherr
    // otherwise it does a third

    // ObjectOrTypeMemberDefinitionMaker actually need to merge with MemberMaker

    // these two share a lot of code
    // but is all boiler plate so I don't care
    // {0CFF70E2-9691-4A79-9327-11385BFA3DC9}
    internal class ObjectOrTypeMemberDefinitionMaker : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    {
        public ObjectOrTypeMemberDefinitionMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            
            var firstTry= tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMaker())
                .Has(new NameMaker())
                .ConvertIfMatched((type,nameToken) => new ObjectOrTypeMemberMemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, Possibly.Is(type)));
            
            if (firstTry is IMatchedTokenMatching) {
                return firstTry;
            }

            return tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken2)
                .Has(new NameMaker())
                .ConvertIfMatched((nameToken) =>
                {
                    return new ObjectOrTypeMemberMemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, Possibly.IsNot<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>());
                });
        }

    }


    internal class ObjectOrTypeMemberMemberDefinitionPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
    {
        private readonly IKey memberName;
        private readonly bool isReadonly;
        private readonly IIsPossibly<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> type;

        public ObjectOrTypeMemberMemberDefinitionPopulateScope(IKey item, bool v, IIsPossibly<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> typeToken)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            isReadonly = v;
            type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
        }

        public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            if (!(scope is Tpn.IHavePublicMembers havePublicMember))
            {
                // this should only be used in object and type definitions 
                throw new NotImplementedException("this should be an ierror");
            }

            var member = this.type.IfElseReturn(x =>
            {
                var type = x.Run(scope, context.CreateChild(this));
                return context.TypeProblem.CreatePublicMember(scope, havePublicMember, memberName, type.SetUpSideNode.TransformInner(x => x.Key()), new WeakMemberDefinitionConverter(isReadonly, memberName));
            },
            () =>
            {
                return context.TypeProblem.CreatePublicMember(scope, havePublicMember, memberName, new WeakMemberDefinitionConverter(isReadonly, memberName));
            });


            return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new ObjectOrTypeMemberMemberDefinitionResolveReferance(
                member), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
        }

    }

    internal class ObjectOrTypeMemberMemberDefinitionResolveReferance : IResolve<IBox<WeakMemberReference>>
    {
        private readonly Tpn.TypeProblem2.Member member;

        public ObjectOrTypeMemberMemberDefinitionResolveReferance(Tpn.TypeProblem2.Member member)
        {
            this.member = member ?? throw new ArgumentNullException(nameof(member));
        }

        public IBox<WeakMemberReference> Run(Tpn.TypeSolution context)
        {
            return new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(member)));
        }
    }
}