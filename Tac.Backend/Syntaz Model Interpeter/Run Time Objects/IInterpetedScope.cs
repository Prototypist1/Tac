using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create();
    }

    public interface IInterpetedScope: IInterpetedAnyType
    {
        // TODO
        // hmmm maybe type should be passed in?
        // Interpeted type???


        // does this really go here?
        // am I getting lazy
        // just throwing things where ever I feel 
        bool ContainsMember(IKey name);
        IInterpetedMember GetMember(IKey name);
        bool TryAddMember(IKey key, IInterpetedMember member);
    }

    public interface IInterpetedStaticScope : IInterpetedScope { }
}