using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class TypeReferance : Referance<TypeDefinition>
    {
        public TypeReferance(NamePath key) : base(key)
        {
        }
        public TypeReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
        {
        }

        public override bool Equals(object obj) => obj is TypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static TypeReferance StringType = new TypeReferance("String");
        public static TypeReferance NumberType = new TypeReferance("Number");
    }
}
