using System;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;

namespace Tac.SemanticModel.CodeStuff
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
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> SubtractMaker = StaticSubtractMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{
    internal class WeakSubtractOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ISubtractOperation>
    {
        public WeakSubtractOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ISubtractOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = SubtractOperation.Create();
            return new BuildIntention<ISubtractOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)), 
                    Right.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal class SubtractOperationMaker : BinaryOperationMaker<WeakSubtractOperation,ISubtractOperation>
    {
        public SubtractOperationMaker() : base(SymbolsRegistry.StaticSubtractSymbol, (l,r)=>
           new Box<WeakSubtractOperation>(
                new WeakSubtractOperation(l,r)), (s, c, l, r) => new OrType<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("number"), new PlaceholderValueConverter())))
        {}
    }
}
