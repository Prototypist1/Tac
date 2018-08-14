using System;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public abstract class AbstractBlockDefinition<TScope> : ICodeElement, IScoped<TScope> where TScope : LocalStaticScope
    {
        protected AbstractBlockDefinition(TScope scope, ICodeElement[] body) {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
        
        public TScope Scope { get; }
        public ICodeElement[] Body { get; }

        public bool ContainsInTree(ICodeElement element) => Equals(element) || Body.Any(x => x.ContainsInTree(element));


    }
}