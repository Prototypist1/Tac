using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendType, ITypeProblemNode>> StaticTypeOrMaker = AddTypeOperationMatcher(() => new TypeOrOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendType, ITypeProblemNode>> TypeOrMaker = StaticTypeOrMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {

        public static string StaticTypeOrSymbol = StaticSymbolsRegistry.AddOrThrow("|");
        public readonly string TypeOrSymbol = StaticTypeOrSymbol;
    }
}

namespace Tac.Frontend._3_Syntax_Model.Operations
{
    internal class WeakTypeOrOperation : BinaryTypeOperation<IFrontendType, IFrontendType, ITypeOr>
    {
        public WeakTypeOrOperation(IIsPossibly<IFrontendType> left, IIsPossibly<IFrontendType> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
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
        public TypeOrOperationMaker() : base(SymbolsRegistry.StaticTypeOrSymbol, (l, r) => 
            Possibly.Is<IWeakTypeReference>(
                new WeakTypeReference(
                    Possibly.Is<IBox<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>>(
                        new Box<IIsPossibly<IConvertableFrontendType<IVerifiableType>>>(
                            Possibly.Is<IConvertableFrontendType<IVerifiableType>>(
                                new WeakTypeOrOperation(l, r)))))),(s,c,l,r)=> {
                                    return c.TypeProblem.CreateOrType(s, (Tpn.ITypeReference)l.SetUpSideNode, (Tpn.ITypeReference)r.SetUpSideNode);
                                })
        {
        }
    }
}
