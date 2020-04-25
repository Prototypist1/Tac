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
        public static readonly string StaticElseSymbol = StaticSymbolsRegistry.AddOrThrow("else");
        public readonly string ElseSymbol = StaticElseSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticElseMaker = AddOperationMatcher(() => new ElseOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ElseMaker = StaticElseMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    // really an if not
    internal class WeakElseOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IElseOperation>, IReturn
    {
        // right should have more validation
        public WeakElseOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IElseOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ElseOperation.Create();
            return new BuildIntention<IElseOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType());
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation,IElseOperation>
    {
        public ElseOperationMaker() : base(SymbolsRegistry.StaticElseSymbol, (l,r)=>new Box<WeakElseOperation>(new WeakElseOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"),new PlaceholderValueConverter())))
        {
        }
    }
    
}
