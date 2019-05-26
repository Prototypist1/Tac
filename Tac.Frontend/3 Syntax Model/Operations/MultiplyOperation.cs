using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    internal class MultiplySymbols : ISymbols
    {
        public string Symbols => "*";
    }

    internal class WeakMultiplyOperation : BinaryOperation<IConvertableFrontendCodeElement<ICodeElement>, IConvertableFrontendCodeElement<ICodeElement>, IMultiplyOperation>
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>> left, IIsPossibly<IConvertableFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateNumberType());
        }

        public override IBuildIntention<IMultiplyOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = MultiplyOperation.Create();
            return new BuildIntention<IMultiplyOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }

    }

    internal class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation, IMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(new MultiplySymbols(), (l,r)=>Possibly.Is(new WeakMultiplyOperation(l,r)))
        {
        }
    }
}
