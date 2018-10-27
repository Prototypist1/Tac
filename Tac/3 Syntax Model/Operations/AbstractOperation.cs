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

    public interface IBinaryOperation<TLeft, TRight>: ICodeElement
    where TLeft :ICodeElement
    where TRight : ICodeElement
    {
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


    public class BinaryOperationMaker<T,TCodeElement> : IOperationMaker<T, TCodeElement>
        where T : class, IWeakCodeElement
    {
        public BinaryOperationMaker(string name, BinaryOperation.Make<TCodeElement> make, Func<TCodeElement, T> makeT
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.makeT = makeT ?? throw new ArgumentNullException(nameof(makeT));
        }

        public string Name { get; }
        private BinaryOperation.Make<TCodeElement> Make { get; }
        private readonly Func<TCodeElement, T> makeT;

        public IResult<IPopulateScope<T, TCodeElement>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<T, TCodeElement>(left, right, Make,makeT));
            }

            return ResultExtension.Bad<IPopulateScope<T, TCodeElement>>();
        }

    }


    public class BinaryPopulateScope<T, TCodeElement> : IPopulateScope<T, TCodeElement>
        where T : IWeakCodeElement
    {
        private readonly IPopulateScope<,IWeakCodeElement> left;
        private readonly IPopulateScope<,IWeakCodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly Func<TCodeElement, T> makeT;
        private readonly DelegateBox<IWeakReturnable> box = new DelegateBox<IWeakReturnable>();

        public BinaryPopulateScope(IPopulateScope<,IWeakCodeElement> left, 
            IPopulateScope<,IWeakCodeElement> right,
            BinaryOperation.Make<TCodeElement> make, 
            Func<TCodeElement, T> makeT)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.makeT = makeT ?? throw new ArgumentNullException(nameof(makeT));
        }

        public IBox<IWeakReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IPopulateBoxes<T, TCodeElement> Run(IPopulateScopeContext context)
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

            return new BinaryResolveReferance<T, TCodeElement>(
                left.Run(context),
                rightres, 
                make, 
                box,
                makeT);
        }
    }



    public class BinaryResolveReferance<T, TCodeElement> : IPopulateBoxes<T, TCodeElement>
        where T : IWeakCodeElement
    {
        public readonly IPopulateBoxes<,IWeakCodeElement> left;
        public readonly IPopulateBoxes<,IWeakCodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly Func<TCodeElement,T > makeT;
        private readonly DelegateBox<IWeakReturnable> box;

        public BinaryResolveReferance(
            IPopulateBoxes<,IWeakCodeElement> resolveReferance1,
            IPopulateBoxes<,IWeakCodeElement> resolveReferance2,
            BinaryOperation.Make<TCodeElement> make,
            DelegateBox<IWeakReturnable> box, 
            Func<TCodeElement, T> makeT)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.makeT = makeT ?? throw new ArgumentNullException(nameof(makeT));
        }
        

        public IOpenBoxes<T,TCodeElement> Run(IResolveReferanceContext context)
        {
            var res = make(
                left.Run(context), 
                right.Run(context));
                box.Set(()=>res.Returns(context.ElementBuilders));
            return new BinaryOpenBoxes<T,TCodeElement>(res,makeT);
        }
    }

    internal class BinaryOpenBoxes<T, TCodeElement> : IOpenBoxes<T, TCodeElement>
    { 
        public TCodeElement CodeElement { get; }
        private Func<TCodeElement, T> makeT;

        public BinaryOpenBoxes(TCodeElement res, Func<TCodeElement, T> makeT)
        {
            this.CodeElement = res;
            this.makeT = makeT ?? throw new ArgumentNullException(nameof(makeT));
        }
    }
}
