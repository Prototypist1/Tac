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

namespace Tac.Semantic_Model.Operations
{

    internal class IfTrueSymbols : ISymbols
    {
        public string Symbols => "then";
    }

    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, IIfOperation>
    {
        // right should have more validation
        public WeakIfTrueOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is<IFrontendType<IVerifiableType>>(PrimitiveTypes.CreateBooleanType());
        }
        
        public override IBuildIntention<IIfOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = IfOperation.Create();
            return new BuildIntention<IIfOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation,IIfOperation>
    {
        public IfTrueOperationMaker() : base(new IfTrueSymbols(), (l,r)=> Possibly.Is(new WeakIfTrueOperation(l,r)))
        {
        }
    }

}
