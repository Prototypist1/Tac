using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create();
    }

    public interface IInterpetedScope: Run_Time_Objects.IInterpeted
    {
        bool ContainsMember(IKey name);
        InterpetedMember GetMember(IKey name);
    }
}