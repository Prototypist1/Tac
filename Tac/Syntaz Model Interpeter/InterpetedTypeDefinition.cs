﻿using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedTypeDefinition: TypeDefinition, IInterpeted, IInterpetedPrimitiveType
    {
        internal static readonly TypeDefinition.Make MakeNew = (scope,key)=> new InterpetedTypeDefinition(scope,key);

        public InterpetedTypeDefinition(IResolvableScope scope, IKey key) : base(scope, key)
        {
        }

        public InterpetedResult Interpet(InterpetedContext interpetedContext)
        {
            // this probably returns a generic type 
        }
    }
}