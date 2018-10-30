using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.Names
{

    public class ImplicitKey : IKey {

    }

    public class GenericNameKey : NameKey
    {
        public GenericNameKey(NameKey name, params IKey[] types) : base(name.Name)
        {
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
        }

        public IKey[] Types { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericNameKey key &&
                   base.Equals(obj) &&
                   Types.SequenceEqual(key.Types);
        }

        public override int GetHashCode()
        {
            var hashCode = -850890288;
            hashCode = (hashCode * -1521134295) + base.GetHashCode();
            hashCode = (hashCode * -1521134295) + Types.Sum(x=>x.GetHashCode());
            return hashCode;
        }
    }

}
