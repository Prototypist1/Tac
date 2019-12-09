using System;
using System.Collections.Generic;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticIfSymbol = StaticSymbolsRegistry.AddOrThrow("then");
        public readonly string IfSymbol = StaticIfSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticIfMaker = AddOperationMatcher(() => new IfTrueOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> IfMaker = StaticIfMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model.Operations
{
    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IIfOperation>
    {
        // right should have more validation
        public WeakIfTrueOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateBooleanType());
        }
        
        public override IBuildIntention<IIfOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = IfOperation.Create();
            return new BuildIntention<IIfOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation,IIfOperation>
    {
        public IfTrueOperationMaker() : base(SymbolsRegistry.StaticIfSymbol, (l,r)=> new Box<WeakIfTrueOperation>(new WeakIfTrueOperation(l,r)), (s, c, l, r) => c.TypeProblem.CreateValue(s, new NameKey("bool"),new PlaceholderValueConverter()))
        {
        }
    }
}
