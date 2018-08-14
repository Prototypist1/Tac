using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class LocalReferance : Referance<LocalDefinition>
    {
        public LocalReferance(NamePath key) : base(key)
        {
        }
        public LocalReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
        {
        }
    }
}
