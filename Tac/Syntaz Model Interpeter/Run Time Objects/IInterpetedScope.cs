using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedScope
    {
        bool ContainsMember(IKey name);
        InterpetedMember GetMember(IKey name);
    }
}