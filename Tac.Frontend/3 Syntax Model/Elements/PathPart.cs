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

    internal class OverlayMemberReference : IWeakMemberReference
    {
        private readonly IWeakMemberReference backing;

        public OverlayMemberReference(IWeakMemberReference backing, Overlay overlay)
        {
            MemberDefinition = backing.MemberDefinition.IfIs(x =>
                Possibly.Is(
                    new DelegateBox<IIsPossibly<IWeakMemberDefinition>>(() =>
                        x.GetValue()
                            .IfIs(z =>
                                Possibly.Is(
                                    new OverlayMemberDefinition(z, overlay))))));
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> MemberDefinition { get; }

        public IBuildIntention<IMemberReferance> GetBuildIntention(IConversionContext context)
        {
            return backing.Cast<IConvertableFrontendCodeElement<IMemberReferance>>().GetBuildIntention(context);
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return backing.Returns();
        }
    }

    internal interface IWeakMemberReference : IConvertableFrontendCodeElement<IMemberReferance>, IFrontendType
    {
        IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> MemberDefinition { get; }
    }

    internal class WeakMemberReference : IWeakMemberReference
    {
        public WeakMemberReference(IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> MemberDefinition { get; }

        public IBuildIntention<IMemberReferance> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MemberReference.Create();
            return new BuildIntention<IMemberReferance>(toBuild, () =>
            {
                maker.Build(MemberDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context));
            });
        }

        public IIsPossibly<IFrontendType> Returns()
        {
            return MemberDefinition.IfIs(x => x.GetValue()).IfIs(x => x.Type).IfIs(x => x.TypeDefinition).IfIs(x => x.GetValue());
        }
    }

    internal class MemberReferanceMaker : IMaker<IPopulateScope<IWeakMemberReference>>
    {
        public MemberReferanceMaker(
            IBox<IIsPossibly<IFrontendType>> lhs)
        {
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IIsPossibly<IFrontendType>> lhs;

        public ITokenMatching<IPopulateScope<IWeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<IWeakMemberReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, new MemberReferancePopulateScope(first.Item, lhs));
            }

            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                    matching.Context);
        }

        public static IPopulateScope<IWeakMemberReference> PopulateScope(string item, IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> lhs)
        {
            return new MemberReferancePopulateScope(item, lhs);
        }
        public static IPopulateBoxes<IWeakMemberReference> PopulateBoxes(string memberName,
                DelegateBox<IIsPossibly<IWeakMemberDefinition>> box,
                IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> lhs)
        {
            return new MemberReferanceResolveReferance(memberName,
                box,
                lhs);
        }

        private class MemberReferancePopulateScope : IPopulateScope<IWeakMemberReference>
        {

            private readonly IBox<IIsPossibly<IFrontendType>> lhs;
            private readonly string memberName;
            private readonly DelegateBox<IIsPossibly<IWeakMemberDefinition>> box = new DelegateBox<IIsPossibly<IWeakMemberDefinition>>();

            public MemberReferancePopulateScope(string item, IBox<IIsPossibly<IFrontendType>> lhs)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IResolvelizeScope<IWeakMemberReference> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                return new MemberReferanceFinalizeScope(memberName, box, lhs);
            }
        }

        private class MemberReferanceFinalizeScope : IResolvelizeScope<IWeakMemberReference>
        {

            private readonly string memberName;
            private readonly IBox<IIsPossibly<IFrontendType>> lhs;
            private readonly DelegateBox<IIsPossibly<IWeakMemberDefinition>> box;

            public MemberReferanceFinalizeScope(
                string memberName,
                DelegateBox<IIsPossibly<IWeakMemberDefinition>> box,
                IBox<IIsPossibly<IFrontendType>> lhs)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
            }

            public IPopulateBoxes<IWeakMemberReference> Run(IResolvableScope resolvableScope, IFinalizeScopeContext context)
            {
                return new MemberReferanceResolveReferance(memberName, box, lhs);
            }
        }

        private class MemberReferanceResolveReferance : IPopulateBoxes<IWeakMemberReference>
        {

            private readonly string memberName;
            private readonly IBox<IIsPossibly<IFrontendType>> lhs;
            private readonly DelegateBox<IIsPossibly<IWeakMemberDefinition>> box;

            public MemberReferanceResolveReferance(
                string memberName,
                DelegateBox<IIsPossibly<IWeakMemberDefinition>> box,
                IBox<IIsPossibly<IFrontendType>> lhs)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
            }

            public IIsPossibly<IWeakMemberReference> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                box.Set(() =>
                {
                    // 😨
                    // TODO  a lot of this could be replaced by IfIs

                    var lshpossible = lhs.GetValue();

                    if (!lshpossible.IsDefinately(out var lshtype, out var nope))
                    {
                        return Possibly.IsNot<WeakMemberDefinition>(nope);
                    }

                    if (lshtype.Value.Is<WeakTypeDefinition>(out var weakTypeDefinition))
                    {
                        if (!(weakTypeDefinition is IScoped interfaceType))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                        }

                        if (interfaceType.Scope.TryGetMember(new NameKey(memberName), false, out var res))
                        {
                            return res.GetValue();
                        }
                        else
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                        }
                    }

                    if (lshtype.Value.Is<WeakMemberReference>(out var memberReference))
                    {
                        if (!memberReference.MemberDefinition.IsDefinately(out var hasMemberDef, out var noMemberDef))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(noMemberDef);
                        }

                        if (!hasMemberDef.Value.GetValue().IsDefinately(out var has, out var hasNot))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(hasNot);
                        }

                        if (!has.Value.Type.IsDefinately(out var has2, out var hasNot2))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(hasNot2);
                        }

                        if (!has2.Value.TypeDefinition.IsDefinately(out var has3, out var hasNot3))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(hasNot3);
                        }

                        if (!has3.Value.GetValue().IsDefinately(out var has4, out var hasNot4))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(hasNot4);
                        }

                        if (!(has4.Value is IScoped interfaceType))
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                        }

                        if (interfaceType.Scope.TryGetMember(new NameKey(memberName), false, out var res))
                        {
                            return res.GetValue();
                        }
                        else
                        {
                            return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                        }
                    }

                    return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                });
                return Possibly.Is(new WeakMemberReference(Possibly.Is(box)));
            }
        }
    }
}