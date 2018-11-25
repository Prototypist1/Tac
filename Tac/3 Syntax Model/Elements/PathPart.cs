using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model
{

    internal class WeakMemberReferance : ICodeElement, IMemberReferance
    {
        public WeakMemberReferance(IBox<WeakMemberDefinition> memberDefinition)
        {
            MemberDefinition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
        }

        public IBox<WeakMemberDefinition> MemberDefinition { get; }

        #region IMemberReferance

        IMemberDefinition IMemberReferance.MemberDefinition => MemberDefinition.GetValue();

        #endregion
        
        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberReferance(this);
        }
        
        public IVarifiableType Returns()
        {
            return MemberDefinition.GetValue();
        }
    }

    internal class MemberReferanceMaker : IMaker<IPopulateScope<WeakMemberReferance>>
    {
        public MemberReferanceMaker(
            IBox<IVarifiableType> lhs)
        {
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        private readonly IBox<IVarifiableType> lhs;

        public ITokenMatching<IPopulateScope<WeakMemberReferance>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new NameMaker(), out var first);
            if (matching is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakMemberReferance>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, new MemberReferancePopulateScope(first.Item, lhs));
            }

            return TokenMatching<IPopulateScope<WeakMemberReferance>>.MakeNotMatch(
                    matching.Context);
        }
    }

    internal class MemberReferancePopulateScope : IPopulateScope< WeakMemberReferance>
    {

        private readonly IBox<IVarifiableType> lhs;
        private readonly string memberName;
        private readonly DelegateBox<WeakMemberDefinition> box = new DelegateBox<WeakMemberDefinition>();

        public MemberReferancePopulateScope( string item, IBox<IVarifiableType> lhs)
        {
            memberName = item ?? throw new ArgumentNullException(nameof(item));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<WeakMemberReferance> Run(IPopulateScopeContext context)
        {

            return new MemberReferanceResolveReferance(memberName, box,lhs);
        }
    }

    internal class MemberReferanceResolveReferance : IPopulateBoxes<WeakMemberReferance>
    {

        private readonly string memberName;
        private readonly IBox<IVarifiableType> lhs;
        private readonly DelegateBox<WeakMemberDefinition> box;

        public MemberReferanceResolveReferance(string memberName, DelegateBox<WeakMemberDefinition> box, IBox<IVarifiableType> lhs)
        {
            this.memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
        }

        public WeakMemberReferance Run(IResolveReferanceContext context)
        {
            box.Set(() =>
            {
                var lshtype = lhs.GetValue();
                if (lshtype is WeakMemberReferance memberReferance && 
                    memberReferance.MemberDefinition.GetValue().Type.TypeDefinition.GetValue() is IInterfaceType interfaceType &&
                    interfaceType.Scope.TryGetMember(new NameKey(memberName), false, out var res)
                ){
                    return res.Cast<WeakMemberDefinition>();
                }
                throw new Exception("something not right");
            });
            return new WeakMemberReferance(box);
        }   
    }
}