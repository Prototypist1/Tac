﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model
{

    public abstract class WeakAbstractBlockDefinition : IWeakCodeElement, IScoped, IWeakReturnable
    {
        protected WeakAbstractBlockDefinition(IWeakFinalizedScope scope, IWeakCodeElement[] body, IEnumerable<IWeakCodeElement> staticInitailizers) {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            StaticInitailizers = staticInitailizers ?? throw new ArgumentNullException(nameof(staticInitailizers));
        }

        public IWeakFinalizedScope Scope { get; }
        public IWeakCodeElement[] Body { get; }
        public IEnumerable<IWeakCodeElement> StaticInitailizers { get; }

        public IWeakReturnable Returns() { return this; }
    }
}