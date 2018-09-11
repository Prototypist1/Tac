﻿using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class LastCallOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LastCallOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }
        
        public override bool Equals(object obj) => obj is LastCallOperation other && base.Equals(other);
        public override int GetHashCode() => base.GetHashCode();
        public override ITypeDefinition<IScope> ReturnType(ScopeStack scope) => throw new NotImplementedException();
    }
}