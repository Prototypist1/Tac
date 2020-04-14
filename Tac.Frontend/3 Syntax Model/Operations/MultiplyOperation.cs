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
using Tac.SemanticModel;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticMultiplySymbols = StaticSymbolsRegistry.AddOrThrow("*");
        public readonly string MultiplySymbols = StaticMultiplySymbols;
    }
}


namespace Tac.Parser
{
    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMultiplyMaker = AddOperationMatcher(() => new MultiplyOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MultiplyMaker = StaticMultiplyMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{
    internal class WeakMultiplyOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IMultiplyOperation>
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IMultiplyOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MultiplyOperation.Create();
            return new BuildIntention<IMultiplyOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)), 
                    Right.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }

    }

    internal class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation, IMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(SymbolsRegistry.StaticMultiplySymbols, (l,r)=>new Box<WeakMultiplyOperation>(new WeakMultiplyOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("number"),new PlaceholderValueConverter())))
        {
        }
    }
}
