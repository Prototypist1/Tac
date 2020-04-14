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
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Prototypist.Toolbox;
using Tac.SemanticModel;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox< IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticLessThanMaker = AddOperationMatcher(() => new LessThanOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> LessThanMaker = StaticLessThanMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticLessThanSymbol = StaticSymbolsRegistry.AddOrThrow("<?");
        public readonly string LessThanSymbol = StaticLessThanSymbol;
    }
    

    internal class WeakLessThanOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILessThanOperation>
    {
        public WeakLessThanOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ILessThanOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = LessThanOperation.Create();
            return new BuildIntention<ILessThanOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)), 
                    Right.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation, ILessThanOperation>
    {
        public LessThanOperationMaker() : base(SymbolsRegistry.StaticLessThanSymbol, (l,r)=> new Box<WeakLessThanOperation>(new WeakLessThanOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"), new PlaceholderValueConverter())))
        {
        }
    }
}
