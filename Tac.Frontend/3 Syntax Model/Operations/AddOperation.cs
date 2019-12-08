using System;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly string StaticAddSymbol = StaticSymbolsRegistry.AddOrThrow("+");
        public readonly string AddSymbol = StaticAddSymbol;
    }

}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticAddMaker = AddOperationMatcher(()=> new AddOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> AddMaker = StaticAddMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model.Operations
{


    internal class WeakAddOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAddOperation>
    {
        public WeakAddOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }

        public override IBuildIntention<IAddOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = AddOperation.Create();
            return new BuildIntention<IAddOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context),Right.GetValue().ConvertElementOrThrow(context));
            });
        }

        public override IIsPossibly<IFrontendType> Returns() {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateNumberType());
        }
    }

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation, IAddOperation>
    {
        public AddOperationMaker() : base(SymbolsRegistry.StaticAddSymbol, (l,r)=>new Box<WeakAddOperation>(new WeakAddOperation(l,r)),(s,c,l,r)=>c.TypeProblem.CreateValue(s, new NameKey("number"),new PlaceholderValueConverter()))
        {
        }
    }

}
