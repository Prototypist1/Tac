using Prototypist.LeftToRight;
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
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticMemberDefinitionMaker = AddElementMakers(
            () => new MemberDefinitionMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> MemberDefinitionMaker = StaticMemberDefinitionMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model
{
    internal class OverlayMemberDefinition: IWeakMemberDefinition
    {
        private readonly IWeakMemberDefinition backing;

        public OverlayMemberDefinition(IWeakMemberDefinition backing, Overlay overlay)
        {
            if (overlay == null)
            {
                throw new ArgumentNullException(nameof(overlay));
            }

            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.Type = backing.Type.IfIs(x => Possibly.Is(new OverlayTypeReference(x,overlay)));
        }

        public IIsPossibly<IWeakTypeReference> Type { get; }
        public bool ReadOnly => backing.ReadOnly;
        public IKey Key=> backing.Key;

        public IMemberDefinition Convert(IConversionContext context)
        {
            return MemberDefinitionShared.Convert(Type,context, ReadOnly,Key);
        }

        public IBuildIntention<IMemberDefinition> GetBuildIntention(IConversionContext context) {
            return MemberDefinitionShared.GetBuildIntention(Type, context, ReadOnly, Key);
        }
        
    }

    // very tac-ian 
    internal static class MemberDefinitionShared {

        public static IMemberDefinition Convert(IIsPossibly<IWeakTypeReference> Type,IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (def, builder) = MemberDefinition.Create();

            var buildIntention = Type.GetOrThrow().TypeDefinition.GetOrThrow().GetValue().GetOrThrow().Cast<IConvertable<IVerifiableType>>().GetBuildIntention(context);
            buildIntention.Build();
            builder.Build(Key, buildIntention.Tobuild, ReadOnly);
            return def;
        }
        public static IBuildIntention<IMemberDefinition> GetBuildIntention(IIsPossibly<IWeakTypeReference> Type, IConversionContext context, bool ReadOnly, IKey Key)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    Type.GetOrThrow().TypeDefinition.GetOrThrow().GetValue().GetOrThrow().ConvertTypeOrThrow(context),
                    ReadOnly);
            });
        }

    }

    internal interface IWeakMemberDefinition:  IConvertable<IMemberDefinition>, IFrontendType
    {
        IIsPossibly<IWeakTypeReference> Type { get; }
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
        public WeakMemberDefinition(bool readOnly, IKey key, IIsPossibly<IWeakTypeReference> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IIsPossibly<IWeakTypeReference> Type { get; }
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

    internal class MemberDefinitionMaker : IMaker<IPopulateScope<WeakMemberReference,Tpn.IMember>>
    {
        public MemberDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .Has(new TypeMaker(), out var type)
                .Has(new NameMaker(), out var nameToken);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new MemberDefinitionPopulateScope(new NameKey(nameToken.Item), readonlyToken != default, type));
            }
            return TokenMatching<IPopulateScope<WeakMemberReference, Tpn.IMember>>.MakeNotMatch(
                               matching.Context);
        }


        public static IPopulateScope<WeakMemberReference, Tpn.IMember> PopulateScope(
            IKey item, 
            bool v, 
            IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> typeToken)
        {
            return new MemberDefinitionPopulateScope(item, v,  typeToken);
        }

        public static IPopulateBoxes<WeakMemberReference> PopulateBoxes(
                IKey memberName,
                bool isReadonly,
                IPopulateBoxes<WeakTypeReference> type)
        {
            return new MemberDefinitionResolveReferance(
                memberName,
                isReadonly,
                type);
        }


        private class MemberDefinitionPopulateScope : IPopulateScope<WeakMemberReference, Tpn.IMember>
        {
            private readonly IKey memberName;
            private readonly bool isReadonly;
            private readonly IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> type;
            
            public MemberDefinitionPopulateScope(IKey item, bool v, IPopulateScope<IWeakTypeReference, Tpn.ITypeReference> typeToken)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                isReadonly = v;
                type = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            }

            public IResolvelizeScope<WeakMemberReference, Tpn.IMember> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {

                var type = this.type.Run(scope, context);
                var member = context.TypeProblem.CreateMember(scope, memberName, type.SetUpSideNode.Key());


                return new MemberDefinitionFinalizeScope(
                    memberName, 
                    isReadonly,
                    type, 
                     
                    member);
            }

        }

        private class MemberDefinitionFinalizeScope : IResolvelizeScope<WeakMemberReference, Tpn.IMember>
        {
            private readonly IKey memberName;
            private readonly bool isReadonly;
            public readonly IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> type;

            public Tpn.IMember SetUpSideNode
            {
                get;
            }

            public MemberDefinitionFinalizeScope(
                IKey memberName,
                bool isReadonly,
                IResolvelizeScope<IWeakTypeReference, Tpn.ITypeReference> type,
                Tpn.IMember member)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.isReadonly = isReadonly;
                this.type = type ?? throw new ArgumentNullException(nameof(type));
                this.SetUpSideNode = member ?? throw new ArgumentNullException(nameof(member));
            }

            public IPopulateBoxes<WeakMemberReference> Run(IResolvableScope scope ,IFinalizeScopeContext context)
            {
                return new MemberDefinitionResolveReferance(
                        memberName,
                        isReadonly,
                        type.Run(scope, context));
            }
        }

        private class MemberDefinitionResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly IKey memberName;
            private readonly bool isReadonly;
            public readonly IPopulateBoxes<IWeakTypeReference> type;

            public MemberDefinitionResolveReferance(
                IKey memberName,
                bool isReadonly,
                IPopulateBoxes<IWeakTypeReference> type)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.isReadonly = isReadonly;
                this.type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                memberDefinitionBox.Fill(
                    Possibly.Is(
                    new WeakMemberDefinition(
                        isReadonly,
                        memberName,
                        type.Run(scope,context))));

                return Possibly.Is(new WeakMemberReference(Possibly.Is(memberDefinitionBox)));
            }
        }
    }

}