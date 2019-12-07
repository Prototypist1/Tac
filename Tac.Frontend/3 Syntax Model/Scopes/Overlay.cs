using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model.Elements;
using static Tac.SyntaxModel.Elements.AtomicTypes.PrimitiveTypes;

namespace Tac.Semantic_Model
{
    internal class Overlay {

        private readonly Dictionary<IGenericTypeParameterPlacholder, IFrontendType> map;

        public Overlay(Dictionary<IGenericTypeParameterPlacholder, IFrontendType> map)
        {
            this.map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public IFrontendType Convert(IFrontendType type) {
            if (type.Is<IWeakTypeDefinition>(out var typeDef))
            {
                return new OverlayTypeDefinition(typeDef,this);
            }
            if (type is IGenericTypeParameterPlacholder generic && map.TryGetValue(generic, out var value)){
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

                if (!overlay.map[key].NullSafeEqual<IGenericTypeParameterPlacholder>(map[key])) {
                    return false;
                }
            }

            foreach (var key in overlay.map.Keys)
            {
                if (!map.ContainsKey(key))
                {
                    return false;
                }

                if (!map[key].NullSafeEqual<IGenericTypeParameterPlacholder>(overlay.map[key]))
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
