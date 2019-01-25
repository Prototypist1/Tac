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
        private readonly IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type;

        public ImplicitMemberMaker( IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type)
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

    }


    internal class ImplicitMemberPopulateScope : IPopulateScope<WeakMemberReference>
    {
        private readonly string memberName;
        private readonly IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type;
        private readonly Box<IIsPossibly<IFrontendType<IVerifiableType>>> box = new Box<IIsPossibly<IFrontendType<IVerifiableType>>>();

        public ImplicitMemberPopulateScope(string item, IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IPopulateBoxes<WeakMemberReference> Run(IPopulateScopeContext context)
        {
            
            IBox< IIsPossibly < WeakMemberDefinition >> memberDef = new Box<IIsPossibly<WeakMemberDefinition>>();

            if (!context.Scope.TryAddMember(DefintionLifetime.Instance,new NameKey(memberName), memberDef))
            {
                throw new Exception("bad bad bad!");
            }


            return new ImplicitMemberResolveReferance(memberName,box,type);
        }


        public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
        {
            return box;
        }


    }

    internal class ImplicitMemberResolveReferance : IPopulateBoxes<WeakMemberReference>
    {
        private readonly Box<IIsPossibly<IFrontendType<IVerifiableType>>> box;
        private readonly string memberName;
        private readonly IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type;

        public ImplicitMemberResolveReferance(
            string memberName,
            Box<IIsPossibly<IFrontendType<IVerifiableType>>> box,
            IBox<IIsPossibly<IFrontendType<IVerifiableType>>> type)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public IIsPossibly<WeakMemberReference> Run(IResolveReferenceContext context)
        {
           return box.Fill(
               Possibly.Is(
                new WeakMemberReference(
                    Possibly.Is(
                        new Box<IIsPossibly<WeakMemberDefinition>>(
                            Possibly.Is(
                                new WeakMemberDefinition(
                                    false, 
                                    new NameKey(memberName),
                                    Possibly.Is(
                                        new WeakTypeReference(
                                            Possibly.Is(type))))))))));
        }
    }
}