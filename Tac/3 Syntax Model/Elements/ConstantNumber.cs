using System;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ConstantNumber : ICodeElement
    {
        public ConstantNumber(double value) 
        {
            Value = value;
        }

        public double Value { get; }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.NumberType);
        }
    }

    public class ConstantNumberMaker : IMaker<ConstantNumber>
    {
        private readonly Func<double, ConstantNumber> make;

        public ConstantNumberMaker(Func<double, ConstantNumber> Make) {
            make = Make ?? throw new ArgumentNullException(nameof(Make));
        }

        public bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<ConstantNumber> result)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                result = new ConstantNumberPopulateScope(dub, make);

                return true;
            }

            result = default;
            return false;
        }

        private class ConstantNumberPopulateScope : IPopulateScope<ConstantNumber>
        {
            private readonly double dub;
            private readonly Func<double, ConstantNumber> make;

            public ConstantNumberPopulateScope(double dub, Func<double, ConstantNumber> Make)
            {
                this.dub = dub;
                make = Make;
            }

            public IResolveReferance<ConstantNumber> Run(IPopulateScopeContext context)
            {
                return new ConstantNumberResolveReferance(dub,make);
            }
        }

        private class ConstantNumberResolveReferance : IResolveReferance<ConstantNumber>
        {
            private readonly double dub;
            private readonly Func<double, ConstantNumber> make;

            public ConstantNumberResolveReferance(double dub, Func<double, ConstantNumber> Make)
            {
                this.dub = dub;
                make = Make;
            }

            public ConstantNumber Run(IResolveReferanceContext context)
            {
                return make(dub);
            }
        }
    }
}
