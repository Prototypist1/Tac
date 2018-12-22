using Prototypist.LeftToRight;
using System;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Semantic_Model
{
    internal class OverlayedScope : IResolvableScope
    {
        private readonly IResolvableScope inner;

        public OverlayedScope(IResolvableScope inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IBuildIntention<IFinalizedScope> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            return inner.GetBuildIntention(context);
        }

        public bool TryGetMember(IKey name, bool staticOnly, out IBox<IIsPossibly<IWeakMemberDefinition>> box)
        {
            box = default;
            if (!inner.TryGetMember(name, staticOnly, out var member)){
                return false;
            }
            box = new DelegateBox<IIsPossibly<IWeakMemberDefinition>>(() =>
            {
                return member.GetValue().IfIs(x =>  Possibly.Is(new OverlayMemberDefinition(x)));
            });
            return true;
        }

        public bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType<IVarifiableType>>> box)
        {
            box = default;
            if (!inner.TryGetType(name, out var type))
            {
                return false;
            }
            box = new DelegateBox<IIsPossibly<IFrontendType<IVarifiableType>>>(() =>
            {
                return type.GetValue().IfIs( x =>
                {
                    if (x.Is<IWeakTypeDefinition>(out var typeDef))
                    {
                        return Possibly.Is(new OverlayTypeDefinition(typeDef));
                    }
                    return Possibly.Is(x);
                });});
            return true;
        }
    }
}