using System;
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
        public static readonly string StaticSubtractSymbol = StaticSymbolsRegistry.AddOrThrow(" - ");
        public readonly string SubtractSymbol = StaticSubtractSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticSubtractMaker = AddOperationMatcher(() => new SubtractOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> SubtractMaker = StaticSubtractMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{
    internal class WeakSubtractOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ISubtractOperation>
    {
        public WeakSubtractOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ISubtractOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = SubtractOperation.Create();
            return new BuildIntention<ISubtractOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation,ISubtractOperation>
    {
        public SubtractOperationMaker() : base(SymbolsRegistry.StaticSubtractSymbol, (l,r)=>
           new Box<WeakSubtractOperation>(
                new WeakSubtractOperation(l,r)), (s, c, l, r) => c.TypeProblem.CreateValue(s, new NameKey("number"), new PlaceholderValueConverter()))
        {}
    }
}
