namespace Tac.Semantic_Model
{
    public class ImplicitTypeReferance : TypeReferance
    {
        public ImplicitTypeReferance() : base("var")
        {
        }

        public override bool Equals(object obj) => obj is ImplicitTypeReferance && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
