﻿using System;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    internal class InterpetedMemberDefinition: MemberDefinition
    {
        public InterpetedMemberDefinition(bool readOnly, NameKey key, IBox<IReturnable> type) : base(readOnly, key, type)
        {
        }

        internal static MemberDefinition MakeNew(bool readOnly, NameKey key, IBox<IReturnable> type)
        {
            return new InterpetedMemberDefinition(readOnly, key, type);
        }
    }
}