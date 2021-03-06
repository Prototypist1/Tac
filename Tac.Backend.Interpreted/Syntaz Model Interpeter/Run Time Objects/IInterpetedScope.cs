﻿using Tac.Model;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Run_Time_Objects;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create();
    }

    public interface IInterpetedScope: IInterpetedAnyType
    {
        // TODO
        // hmmm maybe type should be passed in?
        // Interpreted type???


        // does this really go here?
        // am I getting lazy
        // just throwing things where ever I feel 
        bool ContainsMember(IKey name);
        IInterpetedMember GetMember(IKey name);
        bool TryAddMember(IKey key, IInterpetedMember member);
    }

    public interface IInterpetedStaticScope : IInterpetedScope { }
}