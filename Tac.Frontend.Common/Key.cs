using System.Linq;

namespace Tac.Model
{

    public class ImplicitKey : IKey {

    }

    public class GenericNameKey : IKey
    {
        public readonly  NameKey name;
        public GenericNameKey(NameKey name, IKey[] types)
        {
            this.name = name ?? throw new System.ArgumentNullException(nameof(name));
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

        public override string ToString()
        {
            return $"{nameof(GenericNameKey)}-{name.ToString()}-{Types.Aggregate("",(x,y)=> x +""+ y.ToString())}";
        }
    }
}
