using System;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public abstract class AbstractBlockDefinition<TScope> : CodeElement, IScoped<TScope> where TScope : LocalStaticScope
    {
        protected AbstractBlockDefinition(TScope scope, CodeElement[] body) {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
        
        public TScope Scope { get; }
        public CodeElement[] Body { get; }
    }
}