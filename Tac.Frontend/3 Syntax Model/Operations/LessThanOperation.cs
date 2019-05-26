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

namespace Tac.Semantic_Model.CodeStuff
{
    internal class LessThenSymbols : ISymbols
    {
        public string Symbols => "<?";
    }
    
    internal class WeakLessThanOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILessThanOperation>
    {
        public const string Identifier = "<?";

        public WeakLessThanOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IConvertableFrontendType<IVerifiableType>>(PrimitiveTypes.CreateBooleanType());
        }
        
        public override IBuildIntention<ILessThanOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = LessThanOperation.Create();
            return new BuildIntention<ILessThanOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertOrThrow(context), Right.GetOrThrow().ConvertOrThrow(context));
            });
        }
    }

    internal class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation, ILessThanOperation>
    {
        public LessThanOperationMaker() : base(new LessThenSymbols(), (l,r)=> Possibly.Is(new WeakLessThanOperation(l,r)))
        {
        }
    }
}
