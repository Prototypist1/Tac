using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement>> StaticConstantNumberMaker = AddElementMakers(
            () => new ConstantNumberMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement>>(typeof(MemberMaker)));

#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement>> ConstantNumberMaker = StaticConstantNumberMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}



namespace Tac.Semantic_Model.Operations
{

    internal class WeakConstantNumber : IConvertableFrontendCodeElement<IConstantNumber>
    {
        public WeakConstantNumber(IIsPossibly<double> value) 
        {
            Value = value;
        }

        public IIsPossibly<double> Value { get; }

        public IBuildIntention<IConstantNumber> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ConstantNumber.Create();
            return new BuildIntention<IConstantNumber>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }
        
        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateNumberType());
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

        public static IPopulateScope<WeakConstantNumber> PopulateScope(double dub)
        {
            return new ConstantNumberPopulateScope(dub);
        }
        public static IPopulateBoxes<WeakConstantNumber> PopulateBoxes(double dub)
        {
            return new ConstantNumberResolveReferance(dub);
        }

        private class ConstantNumberPopulateScope : IPopulateScope<WeakConstantNumber>
        {
            private readonly double dub;

            public ConstantNumberPopulateScope(double dub)
            {
                this.dub = dub;
            }

            public IResolvelizeScope<WeakConstantNumber> Run(IDefineMembers scope, IPopulateScopeContext context)
            {
                return new ConstantNumberFinalizeScope(dub);
            }
        }

        private class ConstantNumberFinalizeScope : IResolvelizeScope<WeakConstantNumber>
        {
            private readonly double dub;

            public ConstantNumberFinalizeScope(double dub)
            {
                this.dub = dub;
            }

            public IPopulateBoxes<WeakConstantNumber> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new ConstantNumberResolveReferance(dub);
            }
        }

        private class ConstantNumberResolveReferance : IPopulateBoxes<WeakConstantNumber>
        {
            private readonly double dub;

            public ConstantNumberResolveReferance(
                double dub)
            {
                this.dub = dub;
            }

            public IIsPossibly<WeakConstantNumber> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakConstantNumber(Possibly.Is(dub)));
            }
        }
    }
}
