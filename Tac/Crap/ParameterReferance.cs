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

        public override bool Equals(object obj) => obj is ParameterReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
