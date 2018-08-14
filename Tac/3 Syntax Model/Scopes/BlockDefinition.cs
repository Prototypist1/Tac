using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public  class BlockDefinition: AbstractBlockDefinition<LocalStaticScope> 
    {
        public BlockDefinition(ICodeElement[] body) : base(new LocalStaticScope(), body) { }
    }
}