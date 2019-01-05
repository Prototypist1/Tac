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
    internal class SubtractSymbols : ISymbols
    {
        public string Symbols => "-";
    }

    internal class WeakSubtractOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, ISubtractOperation>
    {
        public WeakSubtractOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType<IVarifiableType>> Returns()
        {
            return Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType());
        }
        
        public override IBuildIntention<ISubtractOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = SubtractOperation.Create();
            return new BuildIntention<ISubtractOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }
    }

    internal class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation,ISubtractOperation>
    {
        public SubtractOperationMaker() : base(new SubtractSymbols(), (l,r)=>
            Possibly.Is(
                new WeakSubtractOperation(l,r))){}
    }
}
