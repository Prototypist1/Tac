using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<IKey> MemberKeys => inner.MemberKeys;

        public IBuildIntention<IFinalizableScope> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = Model.Instantiated.Scope.Create();
            return new BuildIntention<IFinalizableScope>(toBuild, () =>
            {
                maker.Build(inner.MemberKeys.Select(key => 
                {
                    if (TryGetMember(key, false, out var member)) {
                        return new Tac.Model.Instantiated.Scope.IsStatic(member.GetValue().GetOrThrow().Convert(context), false);
                    }
                    else {
                        throw new Exception("bug");
                    }
                }).ToArray());
            });
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