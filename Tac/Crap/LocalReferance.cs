using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class LocalReferance : Referance<VariableDefinition>
    {
        public LocalReferance(NamePath key) : base(key)
        {
        }
        public LocalReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
        {
        }

        public override bool Equals(object obj) => obj is LocalReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
