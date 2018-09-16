using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{

    public class AddOperation : BinaryOperation<ICodeElement,ICodeElement>
    {
        public AddOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is AddOperation other && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override ITypeDefinition ReturnType(ScopeStack scope) {
            if (left.ReturnType(scope) == RootScope.NumberType && right.ReturnType(scope) == RootScope.NumberType)
            {
                return RootScope.NumberType.GetTypeDefinition(scope);
            }
            else if (left.ReturnType(scope) == RootScope.StringType && right.ReturnType(scope) == RootScope.StringType)
            {
                return RootScope.NumberType.GetTypeDefinition(scope);
            }
            else
            {
                throw new Exception("add expects string and int");
            }
        }
    }
}
