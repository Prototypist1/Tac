using System;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    // really an if not
    public class ElseOperation : BinaryOperation<ICodeElement, ICodeElement>
    {
        // right should have more validation
        public ElseOperation(ICodeElement left, ICodeElement right) : base(left, right)
        {
        }

        public override IBox<ITypeDefinition> ReturnType()
        {
            return scope.GetTypeOrThrow(RootScope.EmptyType);
        }
    }


    public class ElseOperationMaker : BinaryOperationMaker<ElseOperation>
    {
        public ElseOperationMaker(Func<ICodeElement, ICodeElement, ElseOperation> make) : base("else", make)
        {
        }
    }
}
