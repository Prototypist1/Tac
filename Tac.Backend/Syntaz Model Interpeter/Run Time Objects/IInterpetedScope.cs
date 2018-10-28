using Tac.Model;
using Tac.Semantic_Model.Names;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedScope: IRunTime
    {
        bool ContainsMember(IKey name);
        InterpetedMember GetMember(IKey name);
    }
}