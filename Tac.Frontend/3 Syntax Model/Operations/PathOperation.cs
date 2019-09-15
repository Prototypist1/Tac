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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticPathMaker = AddOperationMatcher(() => new PathOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> PathMaker = StaticPathMaker;
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

        public string Symbol { get; } = ".";

        public ITokenMatching<IPopulateScope<WeakPathOperation>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .Has(new BinaryOperationMatcher(Symbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) match);
            if (matching is IMatchedTokenMatching matched)
            {
                var box = new Box<IIsPossibly<IFrontendType>>();
                var left = matching.Context.ParseLine(match.perface);
                var right = matching.Context.ExpectPathPart(box).ParseParenthesisOrElement(match.rhs);

                return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeMatch(
                    matched.Tokens,
                    matched.Context,
                    new WeakPathOperationPopulateScope(left, right, box));
            }

            return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeNotMatch(
                    matching.Context);
        }


        private class WeakPathOperationPopulateScope : IPopulateScope<WeakPathOperation>
        {
            private readonly IPopulateScope<IFrontendCodeElement> left;
            private readonly IPopulateScope<IFrontendCodeElement> right;
            private readonly Box<IIsPossibly<IFrontendType>> leftType;
            private readonly DelegateBox<IIsPossibly<IFrontendType>> box = new DelegateBox<IIsPossibly<IFrontendType>>();

            public WeakPathOperationPopulateScope(IPopulateScope<IFrontendCodeElement> left,
                IPopulateScope<IFrontendCodeElement> right,
                Box<IIsPossibly<IFrontendType>> leftType)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.leftType = leftType ?? throw new ArgumentNullException(nameof(leftType));
            }

            public IBox<IIsPossibly<IFrontendType>> GetReturnType()
            {
                return box;
            }

            public IResolvelizeScope<WeakPathOperation> Run(IPopulatableScope scope, IPopulateScopeContext context)
            {
                // TODO
                // this is something I don't much like
                // right runs first because of assign
                // in assign you might have something like
                // method [int;int] input { input <? 2 if { 1 return; } else { input - 1 > fac * input return; } } =: fac
                // if the left runs first than fac will not be found
                // and so it will add it to the scope
                // but if the right is run first 
                // fac works
                // if I add an assign that goes the other way...
                // this will break

                // part of me just thinks 
                // force 'var' on member definition 
                // of this.
                // or force all scopes to name themselves and use name.

                // but.. this does not even work
                // cuz now this does not work
                // fac =: method [int;int] input { input <? 2 if { 1 return; } else { input - 1 > fac * input return; } }

                // maybe we need a keywork
                // var or new or make

                return new WeakPathOperationFinalizeScope(
                    left.Run(scope, context),
                    right.Run(scope, context),
                    box,
                    leftType);
            }
        }


        private class WeakPathOperationFinalizeScope : IResolvelizeScope<WeakPathOperation>
        {
            public readonly IResolvelizeScope<IFrontendCodeElement> left;
            public readonly IResolvelizeScope<IFrontendCodeElement> right;
            private readonly DelegateBox<IIsPossibly<IFrontendType>> box;
            private readonly Box<IIsPossibly<IFrontendType>> leftType;

            public WeakPathOperationFinalizeScope(
                IResolvelizeScope<IFrontendCodeElement> resolveReferance1,
                IResolvelizeScope<IFrontendCodeElement> resolveReferance2,
                DelegateBox<IIsPossibly<IFrontendType>> box,
                Box<IIsPossibly<IFrontendType>> leftType)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.leftType = leftType ?? throw new ArgumentNullException(nameof(leftType));
            }


            public IPopulateBoxes<WeakPathOperation> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new WeakPathOperationResolveReferance(left.Run(parent, context), right.Run(parent, context), box, leftType);
            }
        }

        private class WeakPathOperationResolveReferance : IPopulateBoxes<WeakPathOperation>
        {
            public readonly IPopulateBoxes<IFrontendCodeElement> left;
            public readonly IPopulateBoxes<IFrontendCodeElement> right;
            private readonly DelegateBox<IIsPossibly<IFrontendType>> box;
            private readonly Box<IIsPossibly<IFrontendType>> leftType;

            public WeakPathOperationResolveReferance(
                IPopulateBoxes<IFrontendCodeElement> resolveReferance1,
                IPopulateBoxes<IFrontendCodeElement> resolveReferance2,
                DelegateBox<IIsPossibly<IFrontendType>> box,
                Box<IIsPossibly<IFrontendType>> leftType)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
                this.box = box ?? throw new ArgumentNullException(nameof(box));
                this.leftType = leftType ?? throw new ArgumentNullException(nameof(leftType));
            }


            public IIsPossibly<WeakPathOperation> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                var leftRes = left.Run(scope, context);
                leftType.Fill(leftRes.IfIs(x => x.Returns()));
                var res = Possibly.Is(new WeakPathOperation(
                    leftRes,
                    right.Run(scope, context)));
                box.Set(() => {
                    if (res.IsDefinately(out var yes, out var no))
                    {
                        return yes.Value.Returns();
                    }
                    else
                    {
                        return Possibly.IsNot<IConvertableFrontendType<IVerifiableType>>(no);
                    }
                });
                return res;
            }
        }
    }


    //internal class PathOperationMaker : IMaker<IPopulateScope<WeakPathOperation>>
    //{
    //    public ITokenMatching<IPopulateScope<WeakPathOperation>> TryMake(IMatchedTokenMatching tokenMatching)
    //    {
    //        var matching = tokenMatching
    //            .Has(new BinaryOperationMatcher(SymbolsRegistry.StaticPathSymbol), out (IReadOnlyList<IToken> perface, AtomicToken token, IToken rhs) res);
    //        if (matching is IMatchedTokenMatching matched)
    //        {
    //            var left = matching.Context.ParseLine(res.perface);
    //            var right = matching.Context.ExpectPathPart(left.GetReturnType()).ParseParenthesisOrElement(res.rhs);

    //            return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeMatch(
    //                matched.Tokens,
    //                matched.Context,
    //                BinaryOperationMaker<WeakPathOperation, IPathOperation >.PopulateScope(left, right, (l,r)=> Possibly.Is(new WeakPathOperation(l,r))));
    //        }

    //        return TokenMatching<IPopulateScope<WeakPathOperation>>.MakeNotMatch(
    //                matching.Context);
    //    }
    //}
}
