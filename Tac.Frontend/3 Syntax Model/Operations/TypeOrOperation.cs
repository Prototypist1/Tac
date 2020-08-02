using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Prototypist.Toolbox.Object;
using Prototypist.Toolbox;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendType>, Tpn.ITypeProblemNode>> StaticTypeOrMaker = AddTypeOperationMatcher(() => new TypeOrOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendType>, Tpn.ITypeProblemNode>> TypeOrMaker = StaticTypeOrMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {

        public readonly static string StaticTypeOrSymbol = StaticSymbolsRegistry.AddOrThrow("|");
        public readonly string TypeOrSymbol = StaticTypeOrSymbol;
    }
}

namespace Tac.Frontend.SyntaxModel.Operations
{
    // what even is the point of this? it just defers to the type
    internal class WeakTypeOrOperation : BinaryTypeOperation<IFrontendType, IFrontendType, ITypeOr>, IFrontendCodeElement//, IIsType
    {
        //private readonly IBox<IOrType<IFrontendType, IError>> left;
        //private readonly IBox<IOrType<IFrontendType, IError>> right;
        private readonly Lazy<FrontEndOrType> lazy;

        public WeakTypeOrOperation(IBox<IOrType<IFrontendType, IError>> left, IBox<IOrType<IFrontendType, IError>> right) : base(left, right)
        {
            //this.left = left ?? throw new ArgumentNullException(nameof(left));
            //this.right = right ?? throw new ArgumentNullException(nameof(right));
            lazy = new Lazy<FrontEndOrType>(()=> new FrontEndOrType(left.GetValue().TransformInner(x => x), right.GetValue().TransformInner(x => x)));
        }

        public override IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        {
            // not sure what I am doing with this ... should it just become a type?

            var (res, builder) = TypeOr.Create();
            return new BuildIntention<ITypeOr>(res, () => builder.Build(
                Left.GetValue().Is1OrThrow().ConvertTypeOrThrow(context),
                Right.GetValue().Is1OrThrow().ConvertTypeOrThrow(context)
                ));
        }

        public FrontEndOrType FrontendType()
        {
            return lazy.Value;
        }
    }

    internal class TypeOrOperationMaker : BinaryTypeMaker
    {
        public TypeOrOperationMaker() : base(
            SymbolsRegistry.StaticTypeOrSymbol, 
            (l, r) => new Box<FrontEndOrType>(new WeakTypeOrOperation(l, r).FrontendType()),
            (s,c,l,r)=> {

                var key = new ImplicitKey(Guid.NewGuid());
                c.TypeProblem.CreateOrType(s, key,l.SetUpSideNode.TransformInner(x=>x.SafeCastTo<Tpn.ITypeProblemNode,Tpn.TypeProblem2.TypeReference>()), r.SetUpSideNode.TransformInner(x => x.SafeCastTo<Tpn.ITypeProblemNode, Tpn.TypeProblem2.TypeReference>()), new WeakTypeOrOperationConverter());
                var reference = c.TypeProblem.CreateTypeReference(s, key, new WeakTypeReferenceConverter());
                return reference;
            })
        {
        }
    }
}
