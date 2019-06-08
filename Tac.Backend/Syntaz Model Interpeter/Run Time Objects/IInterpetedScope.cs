using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create();
    }

    internal interface IInterpetedScope: IInterpetedAnyType
    {
        // TODO
        // hmmm maybe type should be passed in?
        // Interpeted type???


        // does this really go here?
        // am I getting lazy
        // just throwing things where ever I feel 
        bool ContainsMember(IKey name);
        IInterpetedMember<T> GetMember<T>(IKey name) where T: IInterpetedAnyType;
        bool TryAddMember<T>(IKey key, IInterpetedMember<T> member) where T : IInterpetedAnyType;
    }

    internal interface IInterpetedStaticScope : IInterpetedScope { }
}