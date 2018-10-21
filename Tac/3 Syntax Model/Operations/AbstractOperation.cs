using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;

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
        public readonly TLeft left;
        public readonly TRight right;
        public ICodeElement[] Operands
        {
            get
            {
                return new ICodeElement[] { left, right };
            }
        }

        public BinaryOperation(TLeft left, TRight right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }
        
        public abstract IReturnable Returns(IElementBuilders elementBuilders);
    }


    public class BinaryOperationMaker<T> : IOperationMaker<T>
        where T : class, ICodeElement
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
        where T : ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly IPopulateScope<ICodeElement> right;
        private readonly BinaryOperation.Make<T> make;
        private readonly DelegateBox<IReturnable> box = new DelegateBox<IReturnable>();

        public BinaryPopulateScope(IPopulateScope<ICodeElement> left, IPopulateScope<ICodeElement> right, BinaryOperation.Make<T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders)
        {
            return box;
        }

        public IResolveReference<T> Run(IPopulateScopeContext context)
        {
            return new BinaryResolveReferance<T>(left.Run(context), right.Run(context), make, box);
        }
    }



    public class BinaryResolveReferance<T> : IResolveReference<T>
        where T : ICodeElement
    {
        public readonly IResolveReference<ICodeElement> left;
        public readonly IResolveReference<ICodeElement> right;
        private readonly BinaryOperation.Make<T> make;
        private readonly DelegateBox<IReturnable> box;

        public BinaryResolveReferance(
            IResolveReference<ICodeElement> resolveReferance1,
            IResolveReference<ICodeElement> resolveReferance2,
            BinaryOperation.Make<T> make,
            DelegateBox<IReturnable> box)
        {
            left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
            right = resolveReferance2 ?? throw new ArgumentNullException(nameof(resolveReferance2));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
            this.box = box ?? throw new ArgumentNullException(nameof(box));
        }
        

        public T Run(IResolveReferanceContext context)
        {
            var res = make(left.Run(context), right.Run(context));
            box.Set(()=>res.Returns(context.ElementBuilders));
            return res;
        }
    }

}
