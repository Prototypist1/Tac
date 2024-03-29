﻿using System;
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
        private static readonly WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> StaticTypeOrMaker = AddTypeOperationMatcher(() => new TypeOrOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.ITypeProblemNode>> TypeOrMaker = StaticTypeOrMaker;
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
    // really pointless
    // the base class is also pointless I think
    // and the call to base is forcing some very ugly code
    //internal class WeakTypeOrOperation //: BinaryTypeOperation<IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>, IFrontendType<IVerifiableType>>, IFrontendCodeElement//, IIsType
    //{
    //    //private readonly IBox<IOrType<IFrontendType<IVerifiableType>, IError>> left;
    //    //private readonly IBox<IOrType<IFrontendType<IVerifiableType>, IError>> right;
    //    private readonly IOrType<IFrontendType<IVerifiableType>, IError> frontEndOr;

    //    public WeakTypeOrOperation(IOrType<IFrontendType<IVerifiableType>, IError> frontEndOr)
    //    //    : base(
    //    //    frontEndOr.SwitchReturns(
    //    //        x=>x.left,
    //    //        x=>new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(x))), 
    //    //    frontEndOr.SwitchReturns(
    //    //        x => x.right,
    //    //        x => new Box<IOrType<IFrontendType<IVerifiableType>, IError>>(OrType.Make<IFrontendType<IVerifiableType>, IError>(x))))
    //    {
    //        //this.left = left ?? throw new ArgumentNullException(nameof(left));
    //        //this.right = right ?? throw new ArgumentNullException(nameof(right));


    //        // I think type solution should be the only thing creating FrontEndOrTypes 
    //        //lazy = new Lazy<FrontEndOrType>(()=> new FrontEndOrType(left.GetValue().TransformInner(x => x), right.GetValue().TransformInner(x => x)));
    //        this.frontEndOr = frontEndOr ?? throw new ArgumentNullException(nameof(frontEndOr));
    //    }

    //    //private class DummyBuildIntention : IBuildIntention<ITypeOr>
    //    //{
    //    //    public DummyBuildIntention(ITypeOr tobuild)
    //    //    {
    //    //        Tobuild = tobuild ?? throw new ArgumentNullException(nameof(tobuild));
    //    //    }

    //    //    public Action Build => () => { };

    //    //    public ITypeOr Tobuild { get; init; }
    //    //}

    //    // this doesn't actually build in to anything
    //    //public override IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
    //    //{
    //    //   // defer to what the type we wrap converts to
    //    //    return new DummyBuildIntention(lazy.Value.Convert(context));

    //    //    // not sure what I am doing with this ... should it just become a type?

    //    //    //var (res, builder) = TypeOr.Create();
    //    //    //return new BuildIntention<ITypeOr>(res, () => builder.Build(
    //    //    //    Left.GetValue().Is1OrThrow().Convert(context),
    //    //    //    Right.GetValue().Is1OrThrow().Convert(context)
    //    //    //    ));
    //    //}

    //    public FrontEndOrType FrontendType()
    //    {
    //        return frontEndOr.Is1OrThrow();
    //    }
    //}

    internal class TypeOrOperationMaker : BinaryTypeMaker
    {
        public TypeOrOperationMaker() : base(
            SymbolsRegistry.StaticTypeOrSymbol, 
            //(l, r) => new Box<FrontEndOrType>(new WeakTypeOrOperation(l, r).FrontendType()),
            (s,c,l,r)=> {
                var key = new ImplicitKey(Guid.NewGuid());
                var orType = c.TypeProblem.CreateOrType(
                    s, 
                    key,
                    l.SetUpSideNode.TransformInner(y=>y.SafeCastTo<Tpn.ITypeProblemNode, Tpn.TypeProblem2.TypeReference>()), 
                    r.SetUpSideNode.TransformInner(y => y.SafeCastTo<Tpn.ITypeProblemNode, Tpn.TypeProblem2.TypeReference>()),
                    new WeakTypeOrOperationConverter());
                var reference = c.TypeProblem.CreateTypeReference(s, key, new WeakTypeReferenceConverter());
                return reference;
            })
        {
        }
    }
}
