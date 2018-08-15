//using Tac.Semantic_Model.Names;

//namespace Tac.Semantic_Model
//{
//    // TOOD static method is not a thing so this is not thing...
//    public class StaticMethodReferance : Referance<StaticMethodDefinition>
//    {
//        public StaticMethodReferance(NamePath key) : base(key)
//        {
//        }
//        public StaticMethodReferance(string key) : this(new NamePath(new AbstractName[] { new ExplicitName(key) }))
//        {
//        }

//        public override bool Equals(object obj) => obj is StaticMethodReferance && base.Equals(obj);
//        public override int GetHashCode() => base.GetHashCode();
//    }
//}
