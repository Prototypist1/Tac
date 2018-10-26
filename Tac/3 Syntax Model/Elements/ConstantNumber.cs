using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public interface IConstantNumber: ICodeElement, IReturnable {
        double Value { get; }
    }

    // TODO how does this work???
    // is it returnable?
    // no
    // it returns a number?
    // one might say all numbers are the same
    // but we do know more about constants
    // I guess maybe there should be a class number extended by constant number?
    // IDK!
    public class WeakConstantNumber : IWeakCodeElement, IWeakReturnable
    {
        public delegate WeakConstantNumber Make(double value);

        public WeakConstantNumber(double value) 
        {
            Value = value;
        }

        public double Value { get; }

        public IWeakReturnable Returns(IElementBuilders elementBuilders)
        {
            return elementBuilders.NumberType();
        }
    }

    public class ConstantNumberMaker : IMaker<WeakConstantNumber>
    {
        private readonly WeakConstantNumber.Make make;

        public ConstantNumberMaker(WeakConstantNumber.Make Make) {
            make = Make ?? throw new ArgumentNullException(nameof(Make));
        }

        public IResult<IPopulateScope<WeakConstantNumber>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new ConstantNumberPopulateScope(dub, make));
            }
            
            return ResultExtension.Bad<IPopulateScope<WeakConstantNumber>>();
        }
    }
    
    public class ConstantNumberPopulateScope : IPopulateScope<WeakConstantNumber>
    {
        private readonly double dub;
        private readonly WeakConstantNumber.Make make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ConstantNumberPopulateScope(double dub, WeakConstantNumber.Make Make)
        {
            this.dub = dub;
            make = Make;
        }

        public IPopulateBoxes<WeakConstantNumber> Run(IPopulateScopeContext context)
        {
            return new ConstantNumberResolveReferance(dub, make,box);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class ConstantNumberResolveReferance : IPopulateBoxes<WeakConstantNumber>
    {
        private readonly double dub;
        private readonly WeakConstantNumber.Make make;
        private readonly Box<IWeakReturnable> box;

        public ConstantNumberResolveReferance(
            double dub,
            WeakConstantNumber.Make Make, 
            Box<IWeakReturnable> box)
        {
            this.dub = dub;
            make = Make ?? throw new ArgumentNullException(nameof(Make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakConstantNumber Run(IResolveReferanceContext context)
        {
            return box.Fill(make(dub));
        }
    }
}
