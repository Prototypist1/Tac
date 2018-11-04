using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    public static class Symbols{
        public static string[] GetSymbols()=> 
            AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ISymbols).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .Select(x=> Activator.CreateInstance(x)
            .Cast<ISymbols>().Symbols)
            .ToArray();
    }

    public interface ISymbols {
        string Symbols { get; }
    }



    public abstract class BinaryOperation
    {
        public delegate T Make<out T>(ICodeElement left, ICodeElement right);
    }

    internal abstract class BinaryOperation<TLeft, TRight> : BinaryOperation, ICodeElement, IOperation, IBinaryOperation<TLeft, TRight>
        where TLeft : class, ICodeElement
        where TRight : class, ICodeElement
    {
        public TLeft Left { get; }
        public TRight Right { get; }

        public ICodeElement[] Operands
        {
            get
            {
                return new ICodeElement[] { Left, Right };
            }
        }

        public BinaryOperation(TLeft left, TRight right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public abstract IVarifiableType Returns();

        public abstract T Convert<T>(IOpenBoxesContext<T> context);
    }


    internal class BinaryOperationMaker<TCodeElement> : IOperationMaker<TCodeElement>
        where TCodeElement : class, ICodeElement
    {

        public BinaryOperationMaker(ISymbols name, BinaryOperation.Make<TCodeElement> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public ISymbols Name { get; }
        private BinaryOperation.Make<TCodeElement> Make { get; }

        public IResult<IPopulateScope<TCodeElement>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name.Symbols), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<TCodeElement>(left, right, Make));
            }

            return ResultExtension.Bad<IPopulateScope<TCodeElement>>();
        }

    }


    internal class BinaryPopulateScope<TCodeElement> : IPopulateScope<TCodeElement>
                where TCodeElement : class, ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly IPopulateScope<ICodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly DelegateBox<IVarifiableType> box = new DelegateBox<IVarifiableType>();

        public BinaryPopulateScope(IPopulateScope<ICodeElement> left,
            IPopulateScope<ICodeElement> right,
            BinaryOperation.Make<TCodeElement> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IVarifiableType> GetReturnType()
        {
            return box;
        }

        public IPopulateBoxes<TCodeElement> Run(IPopulateScopeContext context)
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

            return new BinaryResolveReferance<TCodeElement>(
                left.Run(context),
                rightres,
                make,
                box);
        }
    }



    internal class BinaryResolveReferance<TCodeElement> : IPopulateBoxes<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public readonly IPopulateBoxes<ICodeElement> left;
        public readonly IPopulateBoxes<ICodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly DelegateBox<IVarifiableType> box;

        public BinaryResolveReferance(
            IPopulateBoxes<ICodeElement> resolveReferance1,
            IPopulateBoxes<ICodeElement> resolveReferance2,
            BinaryOperation.Make<TCodeElement> make,
            DelegateBox<IVarifiableType> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }


        public TCodeElement Run(IResolveReferanceContext context)
        {
            var res = make(
                left.Run(context),
                right.Run(context));
            box.Set(() => res.Returns());
            return res;
        }
    }
}
