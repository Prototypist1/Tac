﻿using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Frontend.Parser;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;
using System;

namespace Tac.SemanticModel.Operations
{

    // the syntax for this...
    // really needs to be
    // 5 is number n { ... } 
    // n only exists in the { ... }
    // otherwise you could write 
    // 5 is Cat cat { ... } ; cat.age > some-method
    // and that will error out
    
    internal class WeakTryAssignOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ITryAssignOperation>
    {

        public WeakTryAssignOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<ITryAssignOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = TryAssignOperation.Create();
            return new BuildIntention<ITryAssignOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class TryAssignOperationMaker : IMaker<ISetUp<IBox< WeakTryAssignOperation>, Tpn.IValue>>
    {
        public TryAssignOperationMaker()
        {
        }

        public ITokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {

            var matching = tokenMatching
            .Has(new BinaryOperationMatcher(SymbolsRegistry.TryAssignSymbol), out (IToken perface, AtomicToken token, IToken rhs) res)
            .Has(new BlockDefinitionMaker(), out var block);

            if (matching
                 is IMatchedTokenMatching matched)
            {
                var left = matching.Context.Map.GetGreatestParent(res.perface);
                var right = matching.Context.Map.GetGreatestParent(res.rhs);

                return TokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>>.MakeMatch(
                    matched.AllTokens,
                    matched.Context,
                    new BinaryPopulateScope<WeakTryAssignOperation, ITryAssignOperation>(left, right, (l, r) =>
                       new Box<WeakTryAssignOperation>(
                            new WeakTryAssignOperation(l, r)),
                    (s, c, l, r) => {
                        if (!(s is Tpn.IScope runtimeScope))
                        {
                            throw new NotImplementedException("this should be an IError");
                        }
                        return OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(runtimeScope, new NameKey("bool"), new PlaceholderValueConverter()));
                        },true),
                    matched.StartIndex,
                    matched.EndIndex
                );
            }

            return TokenMatching<ISetUp<IBox<WeakTryAssignOperation>, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }


    }


}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticTryAssignMaker = AddOperationMatcher(() => new TryAssignOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> TryAssignMaker = StaticTryAssignMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.CodeStuff
{
    // maybe some registaration in this page
    // like at the bottum we tell something this is here
    // like wanderer modules 
    public partial class SymbolsRegistry
    {
        public static readonly string TryAssignSymbol = StaticSymbolsRegistry.AddOrThrow("is");
    }
}
