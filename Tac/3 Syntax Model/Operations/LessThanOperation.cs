using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    public class LessThanOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        public LessThanOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IBox<ITypeDefinition> ReturnType()
        {
            return scope.GetTypeOrThrow(RootScope.BooleanType);
        }
    }
    
    public class LessThanOperationMaker : BinaryOperationMaker<LessThanOperation>
    {
        public LessThanOperationMaker(Func<ICodeElement, ICodeElement, LessThanOperation> make) : base("<?", make)
        {
        }
    }
}
