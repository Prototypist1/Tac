using Tac.Semantic_Model.Names;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedScope
    {
        bool ContainsMember(NameKey name);
        object GetMember(NameKey name);
        void SetMember<T>(NameKey name, T value);
    }
}