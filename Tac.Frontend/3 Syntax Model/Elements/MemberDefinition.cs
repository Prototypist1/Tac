using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMemberDefinitionMaker = AddElementMakers(
            () => new MemberDefinitionMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> MemberDefinitionMaker = StaticMemberDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{

    // very tac-ian 
    internal static class MemberDefinitionShared {

        public static IMemberDefinition Convert(IBox<IFrontendType> Type,IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (def, builder) = MemberDefinition.Create();

            //uhh seems bad
            var buildIntention = Type.GetValue().Cast<IConvertable<IVerifiableType>>().GetBuildIntention(context);
            buildIntention.Build();
            builder.Build(Key, buildIntention.Tobuild, ReadOnly);
            return def;
        }
        public static IBuildIntention<IMemberDefinition> GetBuildIntention(IBox<IFrontendType> Type, IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    Type.GetValue().ConvertTypeOrThrow(context),
                    ReadOnly);
            });
        }

    }

    internal interface IWeakMemberDefinition:  IConvertable<IMemberDefinition>, IFrontendType
    {
        IBox<IFrontendType> Type { get; }
        bool ReadOnly { get; }
        IKey Key { get; }
        IMemberDefinition Convert(IConversionContext context);
    }

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    internal class WeakMemberDefinition:  IWeakMemberDefinition
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IBox<IFrontendType> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IBox<IFrontendType> Type { get; }
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

    }

    internal class MemberDefinitionMaker : IMaker<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>
    {
        public MemberDefinitionMaker()
        {
        }
        
        public ITokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMaker(), out var type)
                .Has(new NameMaker(), out var nameToken);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type));
            }
            return TokenMatching<ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>>.MakeNotMatch(
                               matching.Context);
        }


        public static ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member> PopulateScope(
            IKey item, 
            bool v, 
            ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> typeToken)
        {
            return new MemberDefinitionPopulateScope(item, v,  typeToken);
        }



        private class MemberDefinitionPopulateScope : ISetUp<WeakMemberReference, Tpn.TypeProblem2.Member>
        {
            private readonly IKey memberName;
            private readonly bool isReadonly;
            private readonly ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> type;
            
            public MemberDefinitionPopulateScope(IKey item, bool v, ISetUp<IFrontendType, Tpn.TypeProblem2.TypeReference> typeToken)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                isReadonly = v;
                type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            }

            public ISetUpResult<WeakMemberReference, Tpn.TypeProblem2.Member> Run(Tpn.IScope scope, ISetUpContext context)
            {

                var type = this.type.Run(scope, context);
                var member = context.TypeProblem.CreateMember(scope, memberName, type.SetUpSideNode.Key(), new WeakMemberDefinitionConverter(isReadonly,memberName));


                return new SetUpResult<WeakMemberReference, Tpn.TypeProblem2.Member>(new MemberDefinitionResolveReferance(
                    member),member);
            }

        }

        private class MemberDefinitionResolveReferance : IResolve<WeakMemberReference>
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