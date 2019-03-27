using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Model.Elements;

namespace Tac.Semantic_Model
{
    internal class Overlay {

        private readonly Dictionary<GemericTypeParameterPlacholder, IFrontendType<IVerifiableType>> map;

        public Overlay(Dictionary<GemericTypeParameterPlacholder, IFrontendType<IVerifiableType>> map)
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public IFrontendType<IVerifiableType> Convert(IFrontendType<IVerifiableType> type) {
            if (type.Is<IWeakTypeDefinition>(out var typeDef))
            {
                return new OverlayTypeDefinition(typeDef,this);
            }
            if (type is _3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder generic && map.TryGetValue(generic, out var value)){
                return value;
            }
            return type;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Overlay overlay)) {
                return false;
            }

            foreach (var key in this.map.Keys)
            {
                if (!overlay.map.ContainsKey(key)) {
                    return false;
                }

                if (!overlay.map[key].NullSafeEqual<GemericTypeParameterPlacholder>(map[key])) {
                    return false;
                }
            }

            foreach (var key in overlay.map.Keys)
            {
                if (!map.ContainsKey(key))
                {
                    return false;
                }

                if (!map[key].NullSafeEqual<GemericTypeParameterPlacholder>(overlay.map[key]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return map.Sum(x => x.Key.GetHashCode() + x.Value.GetHashCode());
        }
    }

}
