using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Model
{

    public class ImplicitKey : IKey {

    }

    public class GenericNameKey : NameKey
    {
        public GenericNameKey(NameKey name, IKey[] types) : base(name.Name)
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
