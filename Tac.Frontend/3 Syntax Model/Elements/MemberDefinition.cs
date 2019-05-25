using Prototypist.LeftToRight;
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
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    internal class OverlayMemberDefinition: IWeakMemberDefinition
    {
        private readonly IWeakMemberDefinition backing;
        private readonly Overlay overlay;

        public OverlayMemberDefinition(IWeakMemberDefinition backing, Overlay overlay)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            this.Type = backing.Type.IfIs(x => Possibly.Is(new OverlayTypeReferance(x,overlay)));
        }

        public IIsPossibly<IWeakTypeReference> Type { get; }
        public bool ReadOnly => backing.ReadOnly;
        public IKey Key=> backing.Key;

        public IMemberDefinition Convert(TransformerExtensions.ConversionContext context) => backing.Convert(context);
        public IBuildIntention<IMemberDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context) => (backing as IConvertableFrontendCodeElement<IMemberDefinition>).GetBuildIntention(context);
        public IIsPossibly<IConvertableFrontendType<IVerifiableType>> Returns() => backing.Returns();
        IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => (backing as IConvertable<IVerifiableType>).GetBuildIntention(context);
        
    }

    internal interface IWeakMemberDefinition: IConvertableFrontendCodeElement<IMemberDefinition>, IConvertableFrontendType<IVerifiableType>
    {
        IIsPossibly<IWeakTypeReference> Type { get; }
        bool ReadOnly { get; }
        IKey Key { get; }
        IMemberDefinition Convert(TransformerExtensions.ConversionContext context);
    }

    //internal class ExternalMemberDefinition : IWeakMemberDefinition
    //{
    //    private readonly IMemberDefinition memberDefinition;

    //    public IIsPossibly<IWeakTypeReferance> Type { get; }
    //    public bool ReadOnly { get; }
    //    public IKey Key { get; }

    //    public ExternalMemberDefinition(IMemberDefinition memberDefinition, IIsPossibly<IWeakTypeReferance> type, bool readOnly, IKey key) {
    //        this.memberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
    //        Type = type ?? throw new ArgumentNullException(nameof(type));
    //        ReadOnly = readOnly;
    //        Key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    public IMemberDefinition Convert(TransformerExtensions.ConversionContext context)
    //    {
    //        return memberDefinition;
    //    }

    //    public IBuildIntention<IMemberDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
    //    {

    //        return new BuildIntention<IMemberDefinition>(memberDefinition, () => { });
    //    }

    //    public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
    //    {
    //        return Possibly.Is(this);
    //    }

    //    IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    //}

    // it is possible members are single instances with look up
    // up I don't think so
    // it is easier just to have simple value objects
    // it is certaianly true at somepoint we will need a flattened list 
    internal class WeakMemberDefinition:  IWeakMemberDefinition
    {
        public WeakMemberDefinition(bool readOnly, IKey key, IIsPossibly<WeakTypeReference> type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            ReadOnly = readOnly;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IIsPossibly<IWeakTypeReference> Type { get; }
        public bool ReadOnly { get; }
        public IKey Key { get; }

        public IBuildIntention<IMemberDefinition> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = MemberDefinition.Create();
            return new BuildIntention<IMemberDefinition>(toBuild, () =>
            {
                maker.Build(
                    Key,
                    TransformerExtensions.Convert<ITypeReferance>(Type.GetOrThrow(),context),
                    ReadOnly);
            });
        }

        public IMemberDefinition Convert(TransformerExtensions.ConversionContext context)
        {
            var (def,builder) = MemberDefinition.Create();

            var buildIntention = Type.GetOrThrow().Cast<IConvertable<ITypeReferance>>().GetBuildIntention(context);
            buildIntention.Build();
            builder.Build(Key, buildIntention.Tobuild, ReadOnly);
            return def; 
        }

        IBuildIntention<IVerifiableType> IConvertable<IVerifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);

        IIsPossibly<IConvertableFrontendType<IVerifiableType>> IConvertableFrontendCodeElement<IMemberDefinition>.Returns()
        {
            return Possibly.Is(this);
        }
    }

    internal class MemberDefinitionMaker : IMaker<IPopulateScope<WeakMemberReference>>
    {
        public MemberDefinitionMaker()
        {
        }
        
        public ITokenMatching<IPopulateScope<WeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .OptionalHas(new KeyWordMaker("readonly"), out var readonlyToken)
                .HasOne(w => w.Has(new TypeReferanceMaker(), out var _),
                        w => w.Has(new TypeDefinitionMaker(), out var _),
                        out var type)
                .Has(new NameMaker(), out var nameToken);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new MemberDefinitionPopulateScope(nameToken.Item, readonlyToken != default, type));
            }
            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                               matching.Context);
        }


        public static IPopulateScope<WeakMemberReference> PopulateScope(string item, bool v, IPopulateScope<WeakTypeReference> typeToken)
        {
            return new MemberDefinitionPopulateScope(item, v,  typeToken);
        }
        public static IPopulateBoxes<WeakMemberReference> PopulateBoxes(
                string memberName,
                Box<IIsPossibly<WeakMemberReference>> box,
                bool isReadonly,
                IPopulateBoxes<WeakTypeReference> type,
                IResolvableScope scope,
                Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox)
        {
            return new MemberDefinitionResolveReferance(
                memberName,
                box,
                isReadonly,
                type,
                scope,
                memberDefinitionBox);
        }


        private class MemberDefinitionPopulateScope : IPopulateScope<WeakMemberReference>
        {
            private readonly string memberName;
            private readonly bool isReadonly;
            private readonly IPopulateScope<WeakTypeReference> typeName;
            private readonly Box<IIsPossibly<WeakMemberReference>> box = new Box<IIsPossibly<WeakMemberReference>>();
            private readonly Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox = new Box<IIsPossibly<WeakMemberDefinition>>();

            public MemberDefinitionPopulateScope(string item, bool v, IPopulateScope<WeakTypeReference> typeToken)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                isReadonly = v;
                typeName = typeToken ?? throw new ArgumentNullException(nameof(typeToken));
            }

            public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
            {
                var key = new NameKey(memberName);
                if (!context.Scope.TryAddMember(DefintionLifetime.Instance, key, memberDefinitionBox))
                {
                    throw new Exception("bad bad bad!");
                }
                return new MemberDefinitionResolveReferance(memberName, box, isReadonly, typeName.Run(context), context.GetResolvableScope(), memberDefinitionBox);
            }

            public IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> GetReturnType()
            {
                return box;
            }
        }

        private class MemberDefinitionResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly string memberName;
            private readonly Box<IIsPossibly<WeakMemberReference>> box;
            private readonly bool isReadonly;
            public readonly IPopulateBoxes<WeakTypeReference> type;
            private readonly IResolvableScope scope;
            private readonly Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox;

            public MemberDefinitionResolveReferance(
                string memberName,
                Box<IIsPossibly<WeakMemberReference>> box,
                bool isReadonly,
                IPopulateBoxes<WeakTypeReference> type,
                IResolvableScope scope,
                Box<IIsPossibly<WeakMemberDefinition>> memberDefinitionBox)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.isReadonly = isReadonly;
                this.type = type ?? throw new ArgumentNullException(nameof(type));
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.memberDefinitionBox = memberDefinitionBox ?? throw new ArgumentNullException(nameof(memberDefinitionBox));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
            {
                memberDefinitionBox.Fill(
                    Possibly.Is(
                    new WeakMemberDefinition(
                        isReadonly,
                        new NameKey(memberName),
                        type.Run(context))));

                return box.Fill(Possibly.Is(new WeakMemberReference(Possibly.Is(memberDefinitionBox))));
            }
        }
    }

}