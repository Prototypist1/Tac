﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
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
        private static readonly WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> StaticTypeOrMaker = AddTypeOperationMatcher(() => new TypeOrOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendType, Tpn.ITypeProblemNode>> TypeOrMaker = StaticTypeOrMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {

        public readonly static string StaticTypeOrSymbol = StaticSymbolsRegistry.AddOrThrow("|");
        public readonly string TypeOrSymbol = StaticTypeOrSymbol;
    }
}

namespace Tac.Frontend._3_Syntax_Model.Operations
{
    internal class WeakTypeOrOperation : BinaryTypeOperation<IFrontendType, IFrontendType, ITypeOr>
    {
        public WeakTypeOrOperation(IBox<IFrontendType> left, IBox<IFrontendType> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITypeOr> GetBuildIntention(IConversionContext context)
        {
            // not sure what I am doing with this ... should it just become a type?

            var (res, builder) = TypeOr.Create();
            return new BuildIntention<ITypeOr>(res, () => builder.Build(
                Left.GetValue().ConvertTypeOrThrow(context),
                Right.GetValue().ConvertTypeOrThrow(context)
                ));
        }
    }

    internal class TypeOrOperationMaker : BinaryTypeMaker
    {
        public TypeOrOperationMaker() : base(SymbolsRegistry.StaticTypeOrSymbol, (l, r) => 
            new Box<WeakTypeReference>(
                new WeakTypeReference(
                        new Box<WeakTypeOrOperation>(
                                new WeakTypeOrOperation(l, r)))),(s,c,l,r)=> {
                                    var key = new ImplicitKey();
                                    c.TypeProblem.CreateOrType(s, key,(Tpn.TypeProblem2.TypeReference)l.SetUpSideNode, (Tpn.TypeProblem2.TypeReference)r.SetUpSideNode,new WeakTypeOrOperationConverter());
                                    var reference = c.TypeProblem.CreateTypeReference(s, key, new WeakTypeReferenceConverter());
                                    return reference;
                                })
        {
        }
    }
}
