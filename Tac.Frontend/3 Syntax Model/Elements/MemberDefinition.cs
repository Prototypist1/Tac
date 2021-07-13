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
    //internal static class MemberDefinitionShared
    //{

    //    public static IMemberDefinition Convert(IBox<IOrType<IFrontendType, IError>> Type, IConversionContext context, Access access, IKey Key)
    //    {
    //        var (def, builder) = MemberDefinition.Create();

    //        //uhh seems bad
    //        var buildIntention = Type.GetValue().TransformInner(x => x.CastTo<IConvertable<IVerifiableType>>().GetBuildIntention(context));
    //        var built = buildIntention.TransformInner(x => { x.Build(); return x.Tobuild; });
    //        builder.Build(Key, built.Is1OrThrow(), access);
    //        return def;
    //    }
    //    public static IBuildIntention<IMemberDefinition> GetBuildIntention(IBox<IOrType<IFrontendType, IError>> Type, IConversionContext context, Access access, IKey Key)
    //    {
    //        var (toBuild, maker) = MemberDefinition.Create();
    //        return new BuildIntention<IMemberDefinition>(toBuild, () =>
    //        {
    //            maker.Build(
    //                Key,
    //                Type.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
    //                access);
    //        });
    //    }

    //}

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
        public WeakMemberDefinition(Access access, IKey key, IBox<IOrType<IFrontendType<IVerifiableType>, IError>> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Access = access;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IOrType<IFrontendType<IVerifiableType>, IError>> Type { get; }
        public Access Access { get; }
        public IKey Key { get; }

        //public IMemberDefinition Convert(IConversionContext context)
        //{
        //    return MemberDefinitionShared.Convert(Type, context, Access, Key);
        //}

        public IBuildIntention<IMemberDefinition> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    Type.GetValue().Is1OrThrow().Convert(context),
                    Access);
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns()
        {
            return OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.RefType(Type.GetValue().TransformInner(x => x)));
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
                .ConvertIfMatched((type, nameToken) => new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type), tokenMatching);
        }

        //public static ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> PopulateScope(
        //    IKey item, 
        //    bool v, 
        //    ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> typeToken)
        //{
        //    return new MemberDefinitionPopulateScope(item, v,  typeToken);
        //}


    }

    // this one is for when the type was already matched
    internal class MemberDefinitionMakerAlreadyMatched : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    {

        public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            return tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMakerAlreadyMatched())
                .Has(new NameMaker())
                .ConvertIfMatched((type, nameToken) => new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type), tokenMatching);
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
        private readonly Access access;
        private readonly ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> type;

        public MemberDefinitionPopulateScope(IKey item, bool isReadonly, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> typeToken)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            access = isReadonly ? Access.ReadOnly: Access.ReadWrite;
            type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
        }

        public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            var type = this.type.Run(scope, context.CreateChildContext(this));


            var member = context.TypeProblem.CreateMember(scope, memberName, type.SetUpSideNode.TransformInner(x => x.Key()), new WeakMemberDefinitionConverter(access, memberName));


            return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new MemberDefinitionResolveReferance(
                scope, access, memberName), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
        }

    }

    internal class MemberDefinitionResolveReferance : IResolve<IBox<WeakMemberReference>>
    {
        private readonly Tpn.IStaticScope scope;
        private readonly Access access;
        private readonly IKey memberName;

        public MemberDefinitionResolveReferance(Tpn.IStaticScope scope, Access access, IKey memberName)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.access = access;
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
        }

        public IBox<WeakMemberReference> Run(Tpn.TypeSolution context)
        {
            if (context.TryGetMember(scope, memberName, out var member)) {
                return new Box<WeakMemberReference>(new WeakMemberReference(new Box<WeakMemberDefinition>(member.Is1OrThrow()))); // don't love the Is1OrThrow here
            }
            throw new Exception("I don't think this should happen, if it does I guess we need to put an IError somewhere");
        }
    }


    // these two share a lot of code
    // but is all boiler plate so I don't care
    // {0CFF70E2-9691-4A79-9327-11385BFA3DC9}
    //internal class ObjectOrTypeMemberDefinitionMaker : IMaker<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>>
    //{
    //    public ObjectOrTypeMemberDefinitionMaker()
    //    {
    //    }

    //    public ITokenMatching<ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
    //    {
            
    //        var firstTry= tokenMatching
    //            .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
    //            .Has(new TypeMaker())
    //            .Has(new NameMaker())
    //            .ConvertIfMatched((type,nameToken) => new ObjectOrTypeMemberMemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, Possibly.Is(type)));
            
    //        if (firstTry is IMatchedTokenMatching) {
    //            return firstTry;
    //        }

    //        return tokenMatching
    //            .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken2)
    //            .Has(new NameMaker())
    //            .ConvertIfMatched((nameToken) =>
    //            {
    //                return new ObjectOrTypeMemberMemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, Possibly.IsNot<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>>());
    //            });
    //    }

    //}


    //internal class ObjectOrTypeMemberMemberDefinitionPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
    //{
    //    private readonly IKey memberName;
    //    private readonly bool isReadonly;
    //    private readonly IIsPossibly<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> type;

    //    public ObjectOrTypeMemberMemberDefinitionPopulateScope(IKey item, bool v, IIsPossibly<ISetUp<IBox<IFrontendType>, Tpn.TypeProblem2.TypeReference>> typeToken)
    //    {
    //        memberName = item ?? throw new ArgumentNullException(nameof(item));
    //        isReadonly = v;
    //        type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
    //    }

    //    public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
    //    {

    //        if (!(scope is Tpn.IHavePublicMembers havePublicMember))
    //        {
    //            // this should only be used in object and type definitions 
    //            throw new NotImplementedException("this should be an ierror");
    //        }

    //        var member = this.type.IfElseReturn(x =>
    //        {
    //            var type = x.Run(scope, context.CreateChild(this));
    //            return context.TypeProblem.CreatePublicMember(scope, havePublicMember, memberName, type.SetUpSideNode.TransformInner(x => x.Key()), new WeakMemberDefinitionConverter(isReadonly, memberName));
    //        },
    //        () =>
    //        {
    //            return context.TypeProblem.CreatePublicMember(scope, havePublicMember, memberName, new WeakMemberDefinitionConverter(isReadonly, memberName));
    //        });


    //        return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new ObjectOrTypeMemberMemberDefinitionResolveReferance(
    //            member), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
    //    }

    //}

    //internal class ObjectOrTypeMemberMemberDefinitionResolveReferance : IResolve<IBox<WeakMemberReference>>
    //{
    //    private readonly Tpn.TypeProblem2.Member member;

    //    public ObjectOrTypeMemberMemberDefinitionResolveReferance(Tpn.TypeProblem2.Member member)
    //    {
    //        this.member = member ?? throw new ArgumentNullException(nameof(member));
    //    }

    //    public IBox<WeakMemberReference> Run(Tpn.TypeSolution context)
    //    {
    //        return new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(member)));
    //    }
    //}
}