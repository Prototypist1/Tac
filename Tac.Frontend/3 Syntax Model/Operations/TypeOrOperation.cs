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
        private static readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> StaticTypeOrMaker = AddTypeOperationMatcher(() => new TypeOrOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IOrType<IBox<IFrontendType>, IError>, Tpn.ITypeProblemNode>> TypeOrMaker = StaticTypeOrMaker;
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
    internal class WeakTypeOrOperation : BinaryTypeOperation<IFrontendType, IFrontendType, ITypeOr>
    {
        public WeakTypeOrOperation(IOrType<IBox<IFrontendType>,IError> left, IOrType<IBox<IFrontendType>, IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        {
            // not sure what I am doing with this ... should it just become a type?

            var (res, builder) = TypeOr.Create();
            return new BuildIntention<ITypeOr>(res, () => builder.Build(
                Left.TransformInner(x=>x.GetValue().ConvertTypeOrThrow(context)),
                Right.TransformInner(x => x.GetValue().ConvertTypeOrThrow(context))
                ));
        }
    }

    internal class TypeOrOperationMaker : BinaryTypeMaker
    {
        public TypeOrOperationMaker() : base(
            SymbolsRegistry.StaticTypeOrSymbol, 
            (l, r) => OrType.Make<IBox < WeakTypeOrOperation >,IError>(new Box<WeakTypeOrOperation>(new WeakTypeOrOperation(l, r))),
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
