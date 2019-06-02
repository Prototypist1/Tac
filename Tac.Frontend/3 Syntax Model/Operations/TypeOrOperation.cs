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


    internal class WeakTypeOrOperation : BinaryTypeOperation<IFrontendType, IFrontendType, ITypeOr>
    {
        public WeakTypeOrOperation(IIsPossibly<IFrontendType> left, IIsPossibly<IFrontendType> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITypeOr> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            // not sure what I am doing with this ... should it just become a type?

            var (res, builder) = TypeOr.Create();
            return new BuildIntention<ITypeOr>(res, () => builder.Build(
                Left.GetOrThrow().ConvertTypeOrThrow(context),
                Right.GetOrThrow().ConvertTypeOrThrow(context)
                ));
        }
    }

    internal class TypeOrOperationMaker : BinaryTypeMaker
    {
        public TypeOrOperationMaker() : base(new TypeOrSymbols(), (l, r) => 
            Possibly.Is<IWeakTypeReference>(
                new WeakTypeReference(
                    Possibly.Is<IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>>(
                        new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>(
                            Possibly.Is<IConvertableFrontendType<IVerifiableType>>(
                                new WeakTypeOrOperation(l, r)))))))


        //
        {
        }
    }
}
