using System.Collections.Generic;

namespace Tac.Semantic_Model
{
    // static scope??
    // why not??
    public class BlockDefinition : StaticScope
    {
        protected readonly Dictionary<IName, Visiblity<MemberDefinition>> LocalMembers = new Dictionary<IName, Visiblity<MemberDefinition>>();

        public override TReferanced Get<TKey, TReferanced>(TKey key) => throw new System.NotImplementedException();
    }
}