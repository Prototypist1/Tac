using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{
    internal class ImplicitMemberMaker : IMaker<IPopulateScope<WeakMemberReference>>
    {
        private readonly IBox<IIsPossibly<IFrontendType>> type;

        public ImplicitMemberMaker( IBox<IIsPossibly<IFrontendType>> type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }


        public ITokenMatching<IPopulateScope<WeakMemberReference>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new KeyWordMaker("var"), out var _)
                .Has(new NameMaker(), out var first);

            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeMatch(
                matched.Tokens,
                matched.Context,
                new ImplicitMemberPopulateScope(first.Item, type));
            }

            return TokenMatching<IPopulateScope<WeakMemberReference>>.MakeNotMatch(
                matching.Context);
        }

        public static IPopulateScope<WeakMemberReference> PopulateScope(string item, IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> type)
        {
            return new ImplicitMemberPopulateScope(item, type);
        }
        public static IPopulateBoxes<WeakMemberReference> PopulateBoxes(
                string memberName,
                Box<IIsPossibly<IFrontendType>> box,
                IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>> type)
        {
            return new ImplicitMemberResolveReferance(
                memberName,
                box,
                type);
        }

        private class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberReference>
        {
            private readonly string memberName;
            private readonly IBox<IIsPossibly<IFrontendType>> type;
            private readonly Box<IIsPossibly<IFrontendType>> box = new Box<IIsPossibly<IFrontendType>>();

            public ImplicitMemberPopulateScope(string item, IBox<IIsPossibly<IFrontendType>> type)
            {
                memberName = item ?? throw new ArgumentNullException(nameof(item));
                this.type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
            {

                IBox<IIsPossibly<WeakMemberDefinition>> memberDef = new Box<IIsPossibly<WeakMemberDefinition>>();

                if (!context.Scope.TryAddMember(DefintionLifetime.Instance, new NameKey(memberName), memberDef))
                {
                    throw new Exception("bad bad bad!");
                }


                return new ImplicitMemberResolveReferance(memberName, box, type);
            }


            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }


        }

        private class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberReference>
        {
            private readonly Box<IIsPossibly<IFrontendType>> box;
            private readonly string memberName;
            private readonly IBox<IIsPossibly<IFrontendType>> type;

            public ImplicitMemberResolveReferance(
                string memberName,
                Box<IIsPossibly<IFrontendType>> box,
                IBox<IIsPossibly<IFrontendType>> type)
            {
                this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
            {
                var innerRes = new WeakMemberReference(Possibly.Is(
                             new Box<IIsPossibly<WeakMemberDefinition>>(
                                 Possibly.Is(
                                     new WeakMemberDefinition(
                                         false,
                                         new NameKey(memberName),
                                         Possibly.Is(
                                             new WeakTypeReference(
                                                 Possibly.Is(type))))))));

                var res = Possibly.Is(innerRes);

                 box.Fill(res);

                return res;
            }
        }

    }
}