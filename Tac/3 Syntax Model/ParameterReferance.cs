using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class ParameterReferance : Referance<ParameterDefinition>
    {
        public ParameterReferance(NamePath key) : base(key)
        {
        }
        public ParameterReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
        {
        }
    }
}
