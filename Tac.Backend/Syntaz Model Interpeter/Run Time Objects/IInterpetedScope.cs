using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create(InterpetedContext interpetedContext);
    }

    internal interface IInterpetedScope: IRunTime
    {
        bool ContainsMember(IKey name);
        InterpetedMember GetMember(IKey name);
    }
}