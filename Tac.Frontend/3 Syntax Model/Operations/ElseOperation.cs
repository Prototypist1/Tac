using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
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
        public static readonly string StaticElseSymbol = StaticSymbolsRegistry.AddOrThrow("else");
        public readonly string ElseSymbol = StaticElseSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticElseMaker = AddOperationMatcher(() => new ElseOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ElseMaker = StaticElseMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{

    // really an if not
    internal class WeakElseOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IElseOperation>
    {
        // right should have more validation
        public WeakElseOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateBooleanType());
        }
        
        public override IBuildIntention<IElseOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ElseOperation.Create();
            return new BuildIntention<IElseOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation,IElseOperation>
    {
        public ElseOperationMaker() : base(SymbolsRegistry.StaticElseSymbol, (l,r)=>Possibly.Is(new WeakElseOperation(l,r)), (s, c, l, r) => c.TypeProblem.CreateValue(s, new NameKey("bool")))
        {
        }
    }
    
}
