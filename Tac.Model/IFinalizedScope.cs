﻿using System.Collections.Generic;
using Tac.Model.Elements;

namespace Tac.Model
{
    // what even is the point of this interface??
    // I want an alis
    // but is it worth the price?
    

    // TODO some scope has a lot of the same members
    // figure scope interface our
    public interface IFinalizedScope 
    {
        IEnumerable<IKey> MemberKeys { get; }
        bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition box);
        bool TryGetType(IKey name, out IVerifiableType type);
        bool TryGetParent(out IFinalizedScope res); 
    }
    
}