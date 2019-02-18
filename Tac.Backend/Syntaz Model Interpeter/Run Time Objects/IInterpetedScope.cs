using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IInterpetedScopeTemplate
    {
        IInterpetedScope Create();
    }

    public interface IInterpetedScope: IInterpetedData
    {
        // TODO
        // hmmm maybe type should be passed in?
        // Interpeted type???

        bool ContainsMember(IKey name);
        IInterpetedMember<T> GetMember<T>(IKey name);
        bool TryAddMember<T>(IKey key, IInterpetedMember<T> member);
    }
}