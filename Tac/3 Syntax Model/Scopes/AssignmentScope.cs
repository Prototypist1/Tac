using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public class AssignmentScope : IScope
    {
        public AssignmentScope(ITypeDefinition implicitType) => ImplicitType = implicitType ?? throw new ArgumentNullException(nameof(implicitType));

        public ITypeDefinition ImplicitType { get; }

        public bool TryGet(IEnumerable<AbstractName> names, out IReferanced item)
        {
            item = default;
            return false;
        }

        public bool TryGet(ImplicitTypeReferance key, out ITypeDefinition item)
        {
            item = ImplicitType;
            return false;
        }
    }

}