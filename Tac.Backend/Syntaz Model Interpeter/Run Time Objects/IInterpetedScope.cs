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
        // TODO
        // hmmm maybe type should be passed in?
        // Interpeted type???

        bool ContainsMember(IKey name);
        bool ContainsMember<T>(IKey name) where T : IInterpeted;
        IInterpetedMember<T> GetMember<T>(IKey name) where T : IInterpeted;
    }
}