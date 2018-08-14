using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class StaticMethodReferance : Referance<StaticMethodDefinition>
    {
        public StaticMethodReferance(NamePath key) : base(key)
        {
        }
        public StaticMethodReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
        {
        }
    }
}
