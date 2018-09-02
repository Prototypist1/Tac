using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ReturnOperation : ICodeElement
    {
        public ReturnOperation(ICodeElement result)
        {
            Result = result;
        }

        public ICodeElement Result { get; }
        
        public override bool Equals(object obj)
        {
            return obj is ReturnOperation operation &&
                   EqualityComparer<ICodeElement>.Default.Equals(Result, operation.Result);
        }

        public override int GetHashCode() => 1482362596 + EqualityComparer<ICodeElement>.Default.GetHashCode(Result);
        public ITypeDefinition<IScope> ReturnType(ScopeStack scope) => RootScope.EmptyType;
    }
}
