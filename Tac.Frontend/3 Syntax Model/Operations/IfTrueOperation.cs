﻿using System;
using System.Collections.Generic;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticIfSymbol = StaticSymbolsRegistry.AddOrThrow("then");
        public readonly string IfSymbol = StaticIfSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticIfMaker = AddOperationMatcher(() => new IfTrueOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> IfMaker = StaticIfMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IIfOperation>, IReturn
    {
        // right should have more validation
        public WeakIfTrueOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }


        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType());


        public override IBuildIntention<IIfOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = IfOperation.Create();
            return new BuildIntention<IIfOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation,IIfOperation>
    {
        public IfTrueOperationMaker() : base(SymbolsRegistry.StaticIfSymbol, (l,r)=> new Box<WeakIfTrueOperation>(new WeakIfTrueOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"),new PlaceholderValueConverter())))
        {
        }
    }
}
