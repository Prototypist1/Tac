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
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;


namespace Tac.Frontend._3_Syntax_Model.Operations
{

    internal class TypeOrSymbols : ISymbols
    {
        public string Symbols => "|";
    }

    internal class WeakTypeOrOperation : BinaryOperation<IFrontendCodeElement<ICodeElement>, IFrontendCodeElement<ICodeElement>, WeakTypeReference>
    {
        public WeakTypeOrOperation(IIsPossibly<IFrontendCodeElement<ICodeElement>> left, IIsPossibly<IFrontendCodeElement<ICodeElement>> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITypeOrOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = TypeOrOperation.Create();
            return new BuildIntention<ITypeOrOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().Convert(context), Right.GetOrThrow().Convert(context));
            });
        }

        public override IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is<IFrontendType<IVerifiableType>>(PrimitiveTypes.CreateNumberType());
        }
    }

    internal class TypeOrOperationMaker : BinaryOperationMaker<WeakTypeOrOperation, WeakTypeReference>
    {
        public TypeOrOperationMaker() : base(new TypeOrSymbols(), (l, r) => Possibly.Is(new WeakTypeOrOperation(l, r)))
        {
        }
    }
}
