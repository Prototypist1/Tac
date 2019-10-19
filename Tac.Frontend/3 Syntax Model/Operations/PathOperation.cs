using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticPathSymbol = StaticSymbolsRegistry.AddOrThrow(".");
        public readonly string PathSymbol = StaticPathSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ITypeProblemNode>> PathMaker = StaticPathMaker;
#pragma warning restore IDE0052 // Remove unread private members
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
        
        public override IBuildIntention<IPathOperation> GetBuildIntention(IConversionContext context)
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

        public PathOperationMaker()
        {
        }


        public ITokenMatching<IPopulateScope<WeakPathOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var matching2 = matched.Has(new NameMaker(), out var first);
                if (matching2 is IMatchedTokenMatching matched2)
                {
                    
                    var left = matching.Context.ParseLine(match.perface);
                    //var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                    return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeMatch(
                        matched2.Tokens,
                        matched2.Context,
                        new WeakPathOperationPopulateScope(left, first.Item));
                }
            }

            return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : IPopulateScope<WeakPathOperation>
        {
            private readonly IPopulateScope<IFrontendCodeElement> left;
            private readonly string name;

            public WeakPathOperationPopulateScope(IPopulateScope<IFrontendCodeElement> left,
                string name)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public IResolvelizeScope<WeakPathOperation> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                left.GetReturnedType().HasMember(new NameKey(name));

                return new WeakPathOperationFinalizeScope(
                    left.Run(scope, context),
                    name);
            }
        }


        private class WeakPathOperationFinalizeScope : IResolvelizeScope<WeakPathOperation>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement> left;
            private readonly string name;

            public WeakPathOperationFinalizeScope(
                IResolvelizeScope<IFrontendCodeElement> resolveReferance1,
                string name)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }

            public IPopulateBoxes<WeakPathOperation> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new WeakPathOperationResolveReferance(left.Run(parent, context), name);
            }
        }

        private class WeakPathOperationResolveReferance : IPopulateBoxes<WeakPathOperation>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            private readonly string name;

            public WeakPathOperationResolveReferance(
                IPopulateBoxes<IFrontendCodeElement> resolveReferance1, 
                string name)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.name = name ?? throw new ArgumentNullException(nameof(name));
            }


            public IIsPossibly<WeakPathOperation> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                var res = Possibly.Is(new WeakPathOperation(
                    left.Run(scope, context),
                    Possibly.Is(new WeakMemberReference(left.GetReturnedType().GetMemberDefinition(new NameKey(name))))));
                return res;
            }
        }
    }
}
