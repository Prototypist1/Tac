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
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    internal class WeakMemberReference : IFrontendCodeElement<IMemberReferance>, IFrontendType<IVarifiableType>
    {
        public WeakMemberReference(IIsPossibly<IBox<IIsPossibly<WeakMemberDefinition>>> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public  IIsPossibly<IBox<IIsPossibly<WeakMemberDefinition>>> MemberDefinition { get; }

        public IBuildIntention<IMemberReferance> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = MemberReferance.Create();
            return new BuildIntention<IMemberReferance>(toBuild, () =>
            {
                maker.Build(MemberDefinition.GetOrThrow().GetValue().GetOrThrow().Convert(context));
            });
        }

        public IIsPossibly<IFrontendType<IVarifiableType>> Returns()
        {
            return MemberDefinition.IfIs(x => x.GetValue());
        }

        IBuildIntention<IVarifiableType> IConvertable<IVarifiableType>.GetBuildIntention(TransformerExtensions.ConversionContext context) => GetBuildIntention(context);
    }

    internal class MemberReferanceMaker : IMaker<IPopulateScope<WeakMemberReference>>
    {
        public MemberReferanceMaker(
            IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs)
        {
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs;

        public ITokenMatching<IPopulateScope<WeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, new MemberReferancePopulateScope(first.Item, lhs));
            }

            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class MemberReferancePopulateScope : IPopulateScope< WeakMemberReference>
    {

        private readonly IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs;
        private readonly string memberName;
        private readonly DelegateBox<IIsPossibly<WeakMemberDefinition>> box = new DelegateBox<IIsPossibly<WeakMemberDefinition>>();

        public MemberReferancePopulateScope( string item, IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IIsPossibly<IFrontendType<IVarifiableType>>> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance(memberName, box,lhs);
        }
    }

    internal class MemberReferanceResolveReferance : IPopulateBoxes<WeakMemberReference>
    {

        private readonly string memberName;
        private readonly IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs;
        private readonly DelegateBox<IIsPossibly<WeakMemberDefinition>> box;

        public MemberReferanceResolveReferance(
            string memberName, 
            DelegateBox<IIsPossibly<WeakMemberDefinition>> box, 
            IBox<IIsPossibly<IFrontendType<IVarifiableType>>> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
        {
            box.Set(() =>
            {
                // TODO a lot of this could be replaced by IfIs

                var lshpossible = lhs.GetValue();

                if (lshpossible.IsDefinately(out var lshtype, out var nope)){
                    return Possibly.IsNot<WeakMemberDefinition>(nope);
                }

                if (!lshtype.Is<WeakMemberReference>(out var memberReference)) {
                    return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                }

                if (!memberReference.MemberDefinition.IsDefinately(out var hasMemberDef, out var noMemberDef))
                {
                    return Possibly.IsNot<WeakMemberDefinition>(noMemberDef);
                }

                if (!hasMemberDef.Value.GetValue().IsDefinately(out var has, out var hasNot)) {
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

                if (!(has4 is IInterfaceType interfaceType))
                {
                    return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                }

                if (interfaceType.Scope.TryGetMember(new NameKey(memberName), false, out var res)) {
                    return Possibly.Is(res.Cast<WeakMemberDefinition>());
                }
                else
                {
                    return Possibly.IsNot<WeakMemberDefinition>(); // TODO
                }
            });
            return Possibly.Is(new WeakMemberReference(Possibly.Is(box)));
        }   
    }
}