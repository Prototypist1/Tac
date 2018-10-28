using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    internal interface IOperation
    {
        ICodeElement[] Operands { get; }
    }

    public abstract class BinaryOperation
    {
        public delegate T Make<out T>(ICodeElement left, ICodeElement right);
    }

    public abstract class BinaryOperation<TLeft, TRight> : BinaryOperation, ICodeElement, IOperation
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

        public abstract IType Returns();
    }


    public class BinaryOperationMaker<TCodeElement> : IOperationMaker<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly IConverter<TCodeElement> converter;

        public BinaryOperationMaker(string name, BinaryOperation.Make<TCodeElement> make,
            IConverter<TCodeElement> converter
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public string Name { get; }
        private BinaryOperation.Make<TCodeElement> Make { get; }

        public IResult<IPopulateScope<TCodeElement>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                var right = matchingContext.ParseParenthesisOrElement(rhs);

                return ResultExtension.Good(new BinaryPopulateScope<TCodeElement>(left, right, Make, converter));
            }

            return ResultExtension.Bad<IPopulateScope<TCodeElement>>();
        }

    }


    public class BinaryPopulateScope<TCodeElement> : IPopulateScope<TCodeElement>
                where TCodeElement : class, ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly IPopulateScope<ICodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly IConverter<TCodeElement> converter;
        private readonly DelegateBox<IType> box = new DelegateBox<IType>();

        public BinaryPopulateScope(IPopulateScope<ICodeElement> left,
            IPopulateScope<ICodeElement> right,
            BinaryOperation.Make<TCodeElement> make,
            IConverter<TCodeElement> converter)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public IBox<IType> GetReturnType()
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
                box,
                converter);
        }
    }



    public class BinaryResolveReferance<TCodeElement> : IPopulateBoxes<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public readonly IPopulateBoxes<ICodeElement> left;
        public readonly IPopulateBoxes<ICodeElement> right;
        private readonly BinaryOperation.Make<TCodeElement> make;
        private readonly DelegateBox<IType> box;
        private readonly IConverter<TCodeElement> converter;

        public BinaryResolveReferance(
            IPopulateBoxes<ICodeElement> resolveReferance1,
            IPopulateBoxes<ICodeElement> resolveReferance2,
            BinaryOperation.Make<TCodeElement> make,
            DelegateBox<IType> box,
            IConverter<TCodeElement> converter)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }


        public IOpenBoxes<TCodeElement> Run(IResolveReferanceContext context)
        {
            var res = make(
                left.Run(context).CodeElement,
                right.Run(context).CodeElement);
            box.Set(() => res.Returns());
            return new BinaryOpenBoxes<TCodeElement>(res, converter);
        }
    }

    internal class BinaryOpenBoxes<TCodeElement> : IOpenBoxes<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        public TCodeElement CodeElement { get; }
        private readonly IConverter<TCodeElement> converter;

        public BinaryOpenBoxes(TCodeElement res, IConverter<TCodeElement> converter)
        {
            CodeElement = res ?? throw new ArgumentNullException(nameof(res));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public T Run<T>(IOpenBoxesContext<T> context)
        {
            return converter.Convert<T>(context, CodeElement);
        }
    }

    // this is a very interesting pattern
    // you can not make a generic Func
    // so this achieve the goal
    public interface IConverter<TOperation>
    {
        T Convert<T>(IOpenBoxesContext<T> context, TOperation co);
    }
}
