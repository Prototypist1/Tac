using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;
using Tac.Frontend.Parser;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticConstantNumberMaker = AddElementMakers(
            () => new ConstantNumberMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));

#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ConstantNumberMaker = StaticConstantNumberMaker;
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
    }

    internal class ConstantNumberMaker : IMaker<ISetUp<WeakConstantNumber, Tpn.IValue>>
    {
        public ConstantNumberMaker() {}

        public ITokenMatching<ISetUp<WeakConstantNumber, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new NumberMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakConstantNumber, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantNumberPopulateScope(dub));
            }
            return TokenMatching<ISetUp<WeakConstantNumber, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakConstantNumber, Tpn.IValue> PopulateScope(double dub)
        {
            return new ConstantNumberPopulateScope(dub);
        }
        public static IResolve<WeakConstantNumber> PopulateBoxes(double dub)
        {
            return new ConstantNumberResolveReferance(dub);
        }

        private class ConstantNumberPopulateScope : ISetUp<WeakConstantNumber, Tpn.IValue>
        {
            private readonly double dub;

            public ConstantNumberPopulateScope(double dub)
            {
                this.dub = dub;
            }

            public ISetUpResult<WeakConstantNumber, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {

                var value = context.TypeProblem.CreateValue(scope,new NameKey("number"), new PlaceholderValueConverter());
                return new SetUpResult<WeakConstantNumber, Tpn.IValue>(new ConstantNumberResolveReferance(dub),value);
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

            public IBox<WeakConstantNumber> Run(Tpn.ITypeSolution context)
            {
                return new Box<WeakConstantNumber>(new WeakConstantNumber(Possibly.Is(dub)));
            }
        }
    }
}
