using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Model.Elements;

namespace Tac.Semantic_Model
{
    internal class Overlay {

        private readonly Dictionary<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder, IFrontendType<IVerifiableType>> map;

        public Overlay(Dictionary<Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder, IFrontendType<IVerifiableType>> map)
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
    }

}
