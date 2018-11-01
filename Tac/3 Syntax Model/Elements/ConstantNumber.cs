using System;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

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
    internal class WeakConstantNumber : ICodeElement, IVarifiableType, IConstantNumber
    {
        public WeakConstantNumber(double value) 
        {
            Value = value;
        }

        public double Value { get; }


        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.ConstantNumber(this);
        }

        public T Convert<T>(ITypeConverter<T> context)
        {
            throw new NotImplementedException();
        }

        public IVarifiableType Returns()
        {
            return new NumberType();
        }
    }

    internal class ConstantNumberMaker : IMaker<WeakConstantNumber>
    {
        public ConstantNumberMaker() {}

        public IResult<IPopulateScope<WeakConstantNumber>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(elementToken.Tokens)
                .Has(ElementMatcher.IsNumber, out double dub)
                .Has(ElementMatcher.IsDone)
                .IsMatch)
            {
                return ResultExtension.Good(new ConstantNumberPopulateScope(dub));
            }
            
            return ResultExtension.Bad<IPopulateScope<WeakConstantNumber>>();
        }
    }

    internal class ConstantNumberPopulateScope : IPopulateScope<WeakConstantNumber>
    {
        private readonly double dub;
        private readonly Box<IVarifiableType> box = new Box<IVarifiableType>();

        public ConstantNumberPopulateScope(double dub)
        {
            this.dub = dub;
        }

        public IPopulateBoxes<WeakConstantNumber> Run(IPopulateScopeContext context)
        {
            return new ConstantNumberResolveReferance(dub, box);
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }
    }

    internal class ConstantNumberResolveReferance : IPopulateBoxes<WeakConstantNumber>
    {
        private readonly double dub;
        private readonly Box<IVarifiableType> box;

        public ConstantNumberResolveReferance(
            double dub,
            Box<IVarifiableType> box)
        {
            this.dub = dub;
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }

        public WeakConstantNumber Run(IResolveReferanceContext context)
        {
            return box.Fill(new WeakConstantNumber(dub));
        }
    }
    
}
