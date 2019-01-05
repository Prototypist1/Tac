using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Tac.Semantic_Model.Operations
{

    internal class AddSymbols : ISymbols
    {
        public string Symbols=> "+";
    }

    internal class WeakAddOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, IAddOperation>
    {
        public WeakAddOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }

        public override IBuildIntention<IAddOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = AddOperation.Create();
            return new BuildIntention<IAddOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context),Right.GetOrThrow().Convert(context));
            });
        }

        public override IIsPossibly<IFrontendType<IVarifiableType>> Returns() {
            return Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType());
        }
    }

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation, IAddOperation>
    {
        public AddOperationMaker() : base(new AddSymbols(),(l,r)=>Possibly.Is(new WeakAddOperation(l,r)))
        {
        }
    }
}
