using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;

namespace Tac.Semantic_Model.CodeStuff
{
    internal interface IOperation
    {
        IWeakCodeElement[] Operands { get; }
    }

    public interface IBinaryOperation<TLeft, TRight> {
        TLeft Left { get; }
        TLeft Right { get; }
    }

    public abstract class BinaryOperation
    {
        public delegate T Make<out T>(IWeakCodeElement left, IWeakCodeElement right);
    }

    public abstract class BinaryOperation<TLeft, TRight> : BinaryOperation, IWeakCodeElement, IOperation
        where TLeft : class, IWeakCodeElement
        where TRight : class, IWeakCodeElement
    {
        public readonly TLeft left;
        public readonly TRight right;
        public IWeakCodeElement[] Operands
        {
            get
            {
                return new IWeakCodeElement[] { left, right };
            }
        }

        public BinaryOperation(TLeft left, TRight right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public abstract IWeakReturnable Returns(IElementBuilders elementBuilders);
    }


    public class BinaryOperationMaker<T> : IOperationMaker<T>
        where T : class, IWeakCodeElement
    {
        public BinaryOperationMaker(string name, BinaryOperation.Make<T> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Name { get; }
        private BinaryOperation.Make<T> Make { get; }

        public IResult<IPopulateScope<T>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<T>(left, right, Make));
            }

            return ResultExtension.Bad<IPopulateScope<T>>();
        }

    }


    public class BinaryPopulateScope<T> : IPopulateScope<T>
        where T : IWeakCodeElement
    {
        private readonly IPopulateScope<IWeakCodeElement> left;
        private readonly IPopulateScope<IWeakCodeElement> right;
        private readonly BinaryOperation.Make<T> make;
        private readonly DelegateBox<IWeakReturnable> box = new DelegateBox<IWeakReturnable>();

        public BinaryPopulateScope(IPopulateScope<IWeakCodeElement> left, IPopulateScope<IWeakCodeElement> right, BinaryOperation.Make<T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<T> Run(IPopulateScopeContext context)
        {
            // TODO
            // this is something I don't much like
            // right runs first because of assign
            // in assign you might have something like
            // method [int;int] input { input < ? 2 if { 1 return; } else { input - 1 > fac * input return; } } =: fac
            // if the left runs first than fac will not be found
            // and so it will add it to the scope
            // but if the right is run first 
            // fac works
            // if I add an assign that goes the other way...
            // this will break

            // part of me just thinks 
            // force 'var' on member definition 
            var rightres = right.Run(context);

            return new BinaryResolveReferance<T>(
                left.Run(context),
                rightres, 
                make, 
                box);
        }
    }



    public class BinaryResolveReferance<T> : IResolveReference<T>
        where T : IWeakCodeElement
    {
        public readonly IResolveReference<IWeakCodeElement> left;
        public readonly IResolveReference<IWeakCodeElement> right;
        private readonly BinaryOperation.Make<T> make;
        private readonly DelegateBox<IWeakReturnable> box;

        public BinaryResolveReferance(
            IResolveReference<IWeakCodeElement> resolveReferance1,
            IResolveReference<IWeakCodeElement> resolveReferance2,
            BinaryOperation.Make<T> make,
            DelegateBox<IWeakReturnable> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        

        public T Run(IResolveReferanceContext context)
        {
            var res = make(
                left.Run(context), 
                right.Run(context));
                box.Set(()=>res.Returns(context.ElementBuilders));
            return res;
        }
    }

}
