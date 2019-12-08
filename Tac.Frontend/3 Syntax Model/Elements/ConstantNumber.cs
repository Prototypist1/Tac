using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> StaticConstantNumberMaker = AddElementMakers(
            () => new ConstantNumberMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>>(typeof(MemberMaker)));

#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, LocalTpn.ITypeProblemNode>> ConstantNumberMaker = StaticConstantNumberMaker;
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

    internal class ConstantNumberMaker : IMaker<ISetUp<WeakConstantNumber, LocalTpn.IValue>>
    {
        public ConstantNumberMaker() {}

        public ITokenMatching<ISetUp<WeakConstantNumber, LocalTpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new NumberMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakConstantNumber, LocalTpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantNumberPopulateScope(dub));
            }
            return TokenMatching<ISetUp<WeakConstantNumber, LocalTpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakConstantNumber, LocalTpn.IValue> PopulateScope(double dub)
        {
            return new ConstantNumberPopulateScope(dub);
        }
        public static IResolve<WeakConstantNumber> PopulateBoxes(double dub)
        {
            return new ConstantNumberResolveReferance(dub);
        }

        private class ConstantNumberPopulateScope : ISetUp<WeakConstantNumber, LocalTpn.IValue>
        {
            private readonly double dub;

            public ConstantNumberPopulateScope(double dub)
            {
                this.dub = dub;
            }

            public ISetUpResult<WeakConstantNumber, LocalTpn.IValue> Run(LocalTpn.IScope scope, ISetUpContext context)
            {

                var value = context.TypeProblem.CreateValue(scope,new NameKey("number"));
                return new SetUpResult<WeakConstantNumber, LocalTpn.IValue>(new ConstantNumberResolveReferance(dub),value);
            }
        }

        private class ConstantNumberResolveReferance : IResolve<WeakConstantNumber>
        {
            private readonly double dub;

            public ConstantNumberResolveReferance(
                double dub)
            {
                this.dub = dub;
            }

            public IIsPossibly<WeakConstantNumber> Run(IResolveContext context)
            {
                return Possibly.Is(new WeakConstantNumber(Possibly.Is(dub)));
            }
        }
    }
}
