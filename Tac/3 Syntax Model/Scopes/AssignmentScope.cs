using System;
using System.Collections.Generic;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class AssignmentScope : IScope
    {
        public AssignmentScope(ICodeElement leftSide)
        {
            LeftSide = leftSide ?? throw new ArgumentNullException(nameof(leftSide));
        }

        public ICodeElement LeftSide { get; }

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item)
        {
            item = default;
            return false;
        }

        public bool TryGet(ImplicitTypeReferance key, out Func<ScopeScope,ITypeDefinition> item)
        {
            item = LeftSide.ReturnType;
            return true;
        }
    }
}