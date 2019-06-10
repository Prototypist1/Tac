using System;
using System.Collections.Generic;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;


namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticIfSymbol = StaticSymbolsRegistry.AddOrThrow("then");
        public readonly string IfSymbol = StaticIfSymbol;
    }
}


namespace Tac.Semantic_Model.Operations
{
    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IIfOperation>
    {
        // right should have more validation
        public WeakIfTrueOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateBooleanType());
        }
        
        public override IBuildIntention<IIfOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = IfOperation.Create();
            return new BuildIntention<IIfOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation,IIfOperation>
    {
        public IfTrueOperationMaker() : base(SymbolsRegistry.StaticIfSymbol, (l,r)=> Possibly.Is(new WeakIfTrueOperation(l,r)))
        {
        }
    }

}
