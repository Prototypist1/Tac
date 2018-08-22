using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public  class BlockDefinition: AbstractBlockDefinition<LocalStaticScope> 
    {
        public BlockDefinition(ICodeElement[] body, LocalStaticScope scope) : base(scope ?? throw new System.ArgumentNullException(nameof(scope)), body) { }
        
        public override bool Equals(object obj) => obj is BlockDefinition && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}