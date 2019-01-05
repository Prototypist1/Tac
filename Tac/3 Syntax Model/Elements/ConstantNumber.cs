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
    internal class WeakConstantNumber : IFrontendCodeElement<IConstantNumber>
    {
        public WeakConstantNumber(IIsPossibly<double> value) 
        {
            Value = value;
        }

        public IIsPossibly<double> Value { get; }

        public IBuildIntention<IConstantNumber> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = ConstantNumber.Create();
            return new BuildIntention<IConstantNumber>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }
        
        public IIsPossibly<IFrontendType<IVarifiableType>> Returns()
        {
            return Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType());
        }
    }

    internal class ConstantNumberMaker : IMaker<IPopulateScope<WeakConstantNumber>>
    {
        public ConstantNumberMaker() {}

        public ITokenMatching<IPopulateScope<WeakConstantNumber>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new NumberMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantNumber>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantNumberPopulateScope(dub));
            }
            return TokenMatching<IPopulateScope<WeakConstantNumber>>.MakeNotMatch(tokenMatching.Context);
        }
    }

    internal class ConstantNumberPopulateScope : IPopulateScope<WeakConstantNumber>
    {
        private readonly double dub;

        public ConstantNumberPopulateScope(double dub)
        {
            this.dub = dub;
        }

        public IPopulateBoxes<WeakConstantNumber> Run(IPopulateScopeContext context)
        {
            return new ConstantNumberResolveReferance(dub);
        }

        public IBox<IIsPossibly<IFrontendType<IVarifiableType>>> GetReturnType()
        {
            return new Box<IIsPossibly<IFrontendType<IVarifiableType>>>(Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType()));
        }
    }

    internal class ConstantNumberResolveReferance : IPopulateBoxes<WeakConstantNumber>
    {
        private readonly double dub;

        public ConstantNumberResolveReferance(
            double dub)
        {
            this.dub = dub;
        }

        public IIsPossibly<WeakConstantNumber> Run(IResolveReferenceContext context)
        {
            return Possibly.Is(new WeakConstantNumber(Possibly.Is(dub)));
        }
    }
    
}
