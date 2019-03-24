using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using static Tac.Frontend.TransformerExtensions;

namespace Tac.Semantic_Model.Operations
{

    // TODO how does this work???
    // is it returnable?
    // no
    // it returns a number?
    // one might say all numbers are the same
    // but we do know more about constants
    // I guess maybe there should be a class number extended by constant number?
    // IDK!
    internal class WeakConstantBool : IFrontendCodeElement<IConstantBool>
    {
        public WeakConstantBool(IIsPossibly<bool> value)
        {
            Value = value;
        }

        public IIsPossibly<bool> Value { get; }

        public IBuildIntention<IConstantBool> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = ConstantBool.Create();
            return new BuildIntention<IConstantBool>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType());
        }
    }

    internal class ConstantBoolMaker : IMaker<IPopulateScope<WeakConstantBool>>
    {
        public ConstantBoolMaker() { }


        internal class BoolMaker : IMaker<bool>
        {
            public ITokenMatching<bool> TryMake(IMatchedTokenMatching self)
            {
                if (self.Tokens.Any() &&
                    self.Tokens.First() is AtomicToken first)
                {
                    if (first.Item == "true") {
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, true);

                    }
                    else if (first.Item == "false"){
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, false);

                    }

                }

                return TokenMatching<bool>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<IPopulateScope<WeakConstantBool>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new BoolMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantBool>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantBoolPopulateScope(dub));
            }
            return TokenMatching<IPopulateScope<WeakConstantBool>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakConstantBool> PopulateScope(bool dub)
        {
            return new ConstantBoolPopulateScope(dub);
        }
        public static IPopulateBoxes<WeakConstantBool> PopulateBoxes(bool dub)
        {
            return new ConstantBoolResolveReferance(dub);
        }

        private class ConstantBoolPopulateScope : IPopulateScope<WeakConstantBool>
        {
            private readonly bool dub;

            public ConstantBoolPopulateScope(bool dub)
            {
                this.dub = dub;
            }

            public IPopulateBoxes<WeakConstantBool> Run(IPopulateScopeContext context)
            {
                return new ConstantBoolResolveReferance(dub);
            }

            public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
            {
                return new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType()));
            }
        }

        private class ConstantBoolResolveReferance : IPopulateBoxes<WeakConstantBool>
        {
            private readonly bool dub;

            public ConstantBoolResolveReferance(
                bool dub)
            {
                this.dub = dub;
            }

            public IIsPossibly<WeakConstantBool> Run(IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakConstantBool(Possibly.Is(dub)));
            }
        }
    }



}
