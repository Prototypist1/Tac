using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticPathSymbol = StaticSymbolsRegistry.AddOrThrow(".");
        public readonly string PathSymbol = StaticPathSymbol;
    }
}

namespace Tac.Semantic_Model.Operations
{
    internal class WeakPathOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IPathOperation>
    {
        public WeakPathOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            // should this check to see if the left contains the member defined on the rhs?
            return Right.IfIs(x =>  Possibly.Is(x.Cast<WeakMemberReference>()));
        }
        
        public override IBuildIntention<IPathOperation> GetBuildIntention(TransformerExtensions.ConversionContext context)
        {
            var (toBuild, maker) = PathOperation.Create();
            return new BuildIntention<IPathOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }
    
    internal class PathOperationMaker : IMaker<IPopulateScope<WeakPathOperation>>
    {
        public ITokenMatching<IPopulateScope<WeakPathOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                var right = matching.Context.ExpectPathPart(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

                return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    BinaryOperationMaker<WeakPathOperation, IPathOperation >.PopulateScope(left, right, (l,r)=> Possibly.Is(new WeakPathOperation(l,r))));
            }

            return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeNotMatch(
                    matching.Context);
        }
    }
}
