using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model;
using Tac.Semantic_Model;

namespace Tac.New
{

    // pretty sure the scope is a type outline

    internal interface IPopulatableTypeOutline
    {
        void HasInferedMember(NameKey nameKey);
        void AcceptsType(IPopulatableTypeOutline typeOutlinerPopulateScope);
        ITypeOutlinerFinalizeScope Convert();
    }

    internal interface ITypeOutlinerFinalizeScope
    {
        ITypeOutlinerResolveReference Convert();
    }

    internal interface ITypeOutlinerResolveReference
    {
        IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> GetMemberDefinition(NameKey nameKey);
    }

    internal class PopulatableTypeOutline : IPopulatableTypeOutline
    {
        private OrType<PopulatableTypeOutline, PopulatableTypeOutlineHeart> inner = new OrType<PopulatableTypeOutline, PopulatableTypeOutlineHeart>(new PopulatableTypeOutlineHeart());

        private readonly struct AcceptsTypeCall {
            public readonly IPopulatableTypeOutline typeOutlinerPopulateScope;

            public AcceptsTypeCall(IPopulatableTypeOutline typeOutlinerPopulateScope)
            {
                this.typeOutlinerPopulateScope = typeOutlinerPopulateScope ?? throw new ArgumentNullException(nameof(typeOutlinerPopulateScope));
            }
        }

        private readonly struct HasInferedMemberCall
        {
            private readonly NameKey nameKey;

            public HasInferedMemberCall(NameKey nameKey)
            {
                this.nameKey = nameKey ?? throw new ArgumentNullException(nameof(nameKey));
            }
        }

        private class PopulatableTypeOutlineHeart
        {
            public readonly List<AcceptsTypeCall> acceptsTypeCalls = new List<AcceptsTypeCall>();
            public readonly List<HasInferedMemberCall> hasInferedMemberCalls = new List<HasInferedMemberCall>();
        }

        public PopulatableTypeOutline() { }

        public void Consume(PopulatableTypeOutline populatableTypeOutline) {
            if (populatableTypeOutline == null)
            {
                throw new ArgumentNullException(nameof(populatableTypeOutline));
            }

            var other = populatableTypeOutline;
            var orType = populatableTypeOutline.inner;
            while (orType.Is<PopulatableTypeOutline>( out var another)) {
                other = another;
                orType = another.inner;
            }

            var self = this;
            var myOrType = this.inner;
            while (myOrType.Is<PopulatableTypeOutline>(out var another))
            {
                self = another;
                myOrType = another.inner;
            }


            if (orType.Is<PopulatableTypeOutlineHeart>(out var heart) && myOrType.Is<PopulatableTypeOutlineHeart>(out var ourHeart))
            {
                if (heart == ourHeart)
                {
                    return;
                    // maybe throw??
                }
                other.inner = new OrType<PopulatableTypeOutline, PopulatableTypeOutlineHeart>(self);
                ourHeart.acceptsTypeCalls.AddRange(heart.acceptsTypeCalls);
                heart.acceptsTypeCalls.Clear();
                ourHeart.hasInferedMemberCalls.AddRange(heart.hasInferedMemberCalls);
                heart.hasInferedMemberCalls.Clear();
            }
            else {
                throw new Exception("bug");
            }
        }

        public void AcceptsType(IPopulatableTypeOutline typeOutlinerPopulateScope)
        {
            if (inner.Is<PopulatableTypeOutline>(out var another))
            {
                another.AcceptsType(typeOutlinerPopulateScope);
            }
            else if (inner.Is<PopulatableTypeOutlineHeart>(out var heart))
            {
                heart.acceptsTypeCalls.Add(new AcceptsTypeCall(typeOutlinerPopulateScope));
            }
            else {
                throw new Exception("bug: or type should have a thing for this");
            }
        }

        private PopulatableTypeOutlineHeart GetHeart() {
            var myOrType = this.inner;
            while (myOrType.Is<PopulatableTypeOutline>(out var another))
            {
                myOrType = another.inner;
            }
            if (myOrType.Is<PopulatableTypeOutlineHeart>(out var ourHeart))
            {
                return ourHeart;
            }
            else
            {
                throw new Exception("bug");
            }
        }

        public ITypeOutlinerFinalizeScope Convert()
        {
            if (inner.Is<PopulatableTypeOutline>(out var nother))
            {
                return nother.Convert();
            }
            else if (inner.Is<PopulatableTypeOutlineHeart>(out var heart))
            {
                return new TypeOutlinerFinalizeScope(
                                    () => GetHeart().acceptsTypeCalls,
                                    () => GetHeart().hasInferedMemberCalls);
            }
            else
            {
                throw new Exception("bug: or type should have a thing for this");
            }
        }

        public void HasInferedMember(NameKey nameKey)
        {
            if (inner.Is<PopulatableTypeOutline>(out var another))
            {
                another.HasInferedMember(nameKey);
            }
            else if (inner.Is<PopulatableTypeOutlineHeart>(out var heart))
            {
                heart.hasInferedMemberCalls.Add(new HasInferedMemberCall(nameKey));
            }
            else
            {
                throw new Exception("bug: or type should have a thing for this");
            }

        }

        private class TypeOutlinerFinalizeScope : ITypeOutlinerFinalizeScope
        {
            private Func<List<AcceptsTypeCall>> getAcceptsTypeCalls;
            private Func<List<HasInferedMemberCall>> getHasInferedMemberCall;

            public TypeOutlinerFinalizeScope(
                Func<List<AcceptsTypeCall>> getAcceptsTypeCalls, 
                Func<List<HasInferedMemberCall>> getHasInferedMemberCall)
            {
                this.getAcceptsTypeCalls = getAcceptsTypeCalls ?? throw new ArgumentNullException(nameof(getAcceptsTypeCalls));
                this.getHasInferedMemberCall = getHasInferedMemberCall ?? throw new ArgumentNullException(nameof(getHasInferedMemberCall));
            }

            public ITypeOutlinerResolveReference Convert()
            {
                throw new System.NotImplementedException();
            }
        }

        private class TypeOutlinerResolveReference : ITypeOutlinerResolveReference
        {
            public IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> GetMemberDefinition(NameKey nameKey)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}