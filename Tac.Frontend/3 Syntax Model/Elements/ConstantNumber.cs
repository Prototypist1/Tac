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
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> StaticConstantNumberMaker = AddElementMakers(
            () => new ConstantNumberMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>>(typeof(MemberMaker)));

#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, ISetUpSideNode>> ConstantNumberMaker = StaticConstantNumberMaker;
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

    internal class ConstantNumberMaker : IMaker<IPopulateScope<WeakConstantNumber, ISetUpValue>>
    {
        public ConstantNumberMaker() {}

        public ITokenMatching<IPopulateScope<WeakConstantNumber, ISetUpValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new NumberMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantNumber, ISetUpValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantNumberPopulateScope(dub));
            }
            return TokenMatching<IPopulateScope<WeakConstantNumber, ISetUpValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakConstantNumber, ISetUpValue> PopulateScope(double dub)
        {
            return new ConstantNumberPopulateScope(dub);
        }
        public static IPopulateBoxes<WeakConstantNumber> PopulateBoxes(double dub)
        {
            return new ConstantNumberResolveReferance(dub);
        }

        private class ConstantNumberPopulateScope : IPopulateScope<WeakConstantNumber, ISetUpValue>
        {
            private readonly double dub;

            public ConstantNumberPopulateScope(double dub)
            {
                this.dub = dub;
            }

            public IResolvelizeScope<WeakConstantNumber, ISetUpValue> Run(IDefineMembers scope, IPopulateScopeContext context)
            {

                var numberType = context.TypeProblem.CreateTypeReference(new NameKey("number"));
                var value = context.TypeProblem.CreateValue(numberType);
                return new ConstantNumberFinalizeScope(dub, value);
            }
        }

        private class ConstantNumberFinalizeScope : IResolvelizeScope<WeakConstantNumber, ISetUpValue>
        {
            private readonly double dub;

            public ISetUpValue SetUpSideNode { get; }


            public ConstantNumberFinalizeScope(double dub, ISetUpValue setUpValue)
            {
                this.dub = dub;
                this.SetUpSideNode = setUpValue ?? throw new System.ArgumentNullException(nameof(setUpValue));
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
