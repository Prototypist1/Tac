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
    internal static class MemberDefinitionShared {

        public static IMemberDefinition Convert(IOrType<IBox<IFrontendType>, IError> Type,IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (def, builder) = MemberDefinition.Create();

            //uhh seems bad
            var buildIntention = Type.TransformInner(x=>x.GetValue().CastTo<IConvertable<IVerifiableType>>().GetBuildIntention(context));
            var built = buildIntention.TransformInner( x=> { x.Build(); return x.Tobuild; });
            builder.Build(Key, built.Is1OrThrow(), ReadOnly);
            return def;
        }
        public static IBuildIntention<IMemberDefinition> GetBuildIntention(IOrType<IBox<IFrontendType>, IError> Type, IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    Type.Is1OrThrow().GetValue().ConvertTypeOrThrow(context),
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
        public WeakMemberDefinition(bool readOnly, IKey key, IOrType<IBox<IFrontendType>,IError> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IOrType<IBox<IFrontendType>, IError> Type { get; }
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
            return OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.RefType(Type.TransformInner( x => x.GetValue())));
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in Type.SwitchReturns(x=>x.GetValue().Validate(),x=>new[] { x }))
            {
                yield return error;
            }
        }
    }

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
                .ConvertIfMatched((type, nameToken)=> new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type)) ;
        }


        //public static ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> PopulateScope(
        //    IKey item, 
        //    bool v, 
        //    ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> typeToken)
        //{
        //    return new MemberDefinitionPopulateScope(item, v,  typeToken);
        //}

        private class MemberDefinitionPopulateScope : ISetUp<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>
        {
            private readonly IKey memberName;
            private readonly bool isReadonly;
            private readonly ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> type;
            
            public MemberDefinitionPopulateScope(IKey item, bool v, ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.TypeProblem2.TypeReference> typeToken)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                isReadonly = v;
                type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            }

            public ISetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {

                var type = this.type.Run(scope, context);
                var member = context.TypeProblem.CreateMember(scope, memberName, type.SetUpSideNode.TransformInner(x=>x.Key()), new WeakMemberDefinitionConverter(isReadonly,memberName));


                return new SetUpResult<IBox<WeakMemberReference>, Tpn.TypeProblem2.Member>(new MemberDefinitionResolveReferance(
                    member), OrType.Make<Tpn.TypeProblem2.Member, IError>(member));
            }

        }

        private class MemberDefinitionResolveReferance : IResolve<IBox<WeakMemberReference>>
        {
            private readonly Tpn.TypeProblem2.Member member;

            public MemberDefinitionResolveReferance(Tpn.TypeProblem2.Member member)
            {
                this.member = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IBox<WeakMemberReference> Run(Tpn.ITypeSolution context)
            {
                return new Box<WeakMemberReference>(new WeakMemberReference(context.GetMember(member)));
            }
        }
    }

}