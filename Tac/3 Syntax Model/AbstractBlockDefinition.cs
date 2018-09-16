﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{
    public abstract class AbstractBlockDefinition : ICodeElement, IScoped
    {
        protected AbstractBlockDefinition(IScope scope, ICodeElement[] body, IEnumerable<ICodeElement> staticInitailizers) {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }
        
        public IScope Scope { get; }
        public ICodeElement[] Body { get; }
        public IEnumerable<ICodeElement> StaticInitailizers { get; }
        
        public abstract ITypeDefinition ReturnType(ScopeStack scope);
    }
}