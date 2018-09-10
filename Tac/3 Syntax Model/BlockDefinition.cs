using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public  class BlockDefinition: AbstractBlockDefinition 
    {
        public BlockDefinition(ICodeElement[] body, IScope scope, IEnumerable<ICodeElement> staticInitailizers) : base(scope ?? throw new System.ArgumentNullException(nameof(scope)), body, staticInitailizers) { }
        
        public override bool Equals(object obj) => obj is BlockDefinition && base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition ReturnType(ScopeStack scope) => RootScope.EmptyType;
    }
}