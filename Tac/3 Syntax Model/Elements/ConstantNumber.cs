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

    public class ConstantNumberMaker<T> : IMaker<T, WeakConstantNumber>
    {
        private readonly Func<WeakConstantNumber,T> make;

        public ConstantNumberMaker(Func<WeakConstantNumber, T> Make) {
            make = Make ?? throw new ArgumentNullException(nameof(Make));
        }

        public IResult<IPopulateScope<T, WeakConstantNumber>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new ConstantNumberPopulateScope<T>(dub, make));
            }
            
            return ResultExtension.Bad<IPopulateScope<T, WeakConstantNumber>>();
        }
    }
    
    public class ConstantNumberPopulateScope<T> : IPopulateScope<T, WeakConstantNumber>
    {
        private readonly double dub;
        private readonly Func<WeakConstantNumber, T> make;
        private readonly Box<IWeakReturnable> box = new Box<IWeakReturnable>();

        public ConstantNumberPopulateScope(double dub, Func<WeakConstantNumber, T> Make)
        {
            this.dub = dub;
            make = Make;
        }

        public IPopulateBoxes<T, WeakConstantNumber> Run(IPopulateScopeContext context)
        {
            return new ConstantNumberResolveReferance<T>(dub, make,box);
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }
    }

    public class ConstantNumberResolveReferance<T> : IPopulateBoxes<T, WeakConstantNumber>
    {
        private readonly double dub;
        private readonly Func<WeakConstantNumber, T> make;
        private readonly Box<IWeakReturnable> box;

        public ConstantNumberResolveReferance(
            double dub,
             Func<WeakConstantNumber, T> Make, 
            Box<IWeakReturnable> box)
        {
            this.dub = dub;
            make = Make ?? throw new ArgumentNullException(nameof(Make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public IOpenBoxes<T, WeakConstantNumber> Run(IResolveReferanceContext context)
        {
            var item = box.Fill(new WeakConstantNumber(dub));
            return new ConstantNumberOpenBoxes<T>(item,make);
        }
    }

    internal class ConstantNumberOpenBoxes<T> : IOpenBoxes<T, WeakConstantNumber>
    {
        public WeakConstantNumber CodeElement { get; }
        private readonly Func<WeakConstantNumber, T> make;

        public ConstantNumberOpenBoxes(WeakConstantNumber item, Func<WeakConstantNumber, T> make)
        {
            this.CodeElement = item ?? throw new ArgumentNullException(nameof(item));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public T Run(IOpenBoxesContext context)
        {
            return make(CodeElement);
        }
    }
}
