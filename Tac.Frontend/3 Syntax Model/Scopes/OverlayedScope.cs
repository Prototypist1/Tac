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
        private readonly Overlay overlay;

        public OverlayedScope(IResolvableScope inner, Overlay overlay)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
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
                return member.GetValue().IfIs(x =>  Possibly.Is(new OverlayMemberDefinition(x,overlay)));
            });
            return true;
        }

        public bool TryGetType(IKey name, out IBox<IIsPossibly<IFrontendType>> box)
        {
            box = default;
            if (!inner.TryGetType(name, out var type))
            {
                return false;
            }
            box = new DelegateBox<IIsPossibly<IFrontendType>>(() =>
            {
                return type.GetValue().IfIs( x =>
                {
                    if (x.Is<IWeakTypeDefinition>(out var typeDef))
                    {
                        return Possibly.Is(new OverlayTypeDefinition(typeDef, overlay));
                    }
                    return Possibly.Is(x);
                });});
            return true;
        }
    }
}