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

    public abstract class BinaryOperation<TLeft,TRight>: ICodeElement , IOperation
        where TLeft: class, ICodeElement
        where TRight: class, ICodeElement
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

        public abstract IBox<ITypeDefinition> ReturnType(ScopeTree scope);
    }


    public class BinaryOperationMaker<T> : IOperationMaker<T>
        where T: class, ICodeElement
    {
        public BinaryOperationMaker(string name, Func<ICodeElement, ICodeElement, T> make
            )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Name { get; }
        private Func<ICodeElement, ICodeElement, T> Make { get; }

        public bool TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out IPopulateScope<T> result)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                IPopulateScope<ICodeElement> left = matchingContext.ParseLine(perface);
                IPopulateScope<ICodeElement> right = matchingContext.ParseParenthesisOrElement(rhs);

                result = new BinaryPopulateScope<T>(left,right, Make);
                return true;
            }

            result = default;
            return false;
        }

    }


    public class BinaryPopulateScope<T> : IPopulateScope<T> 
        where T : ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly IPopulateScope<ICodeElement> right;
        private readonly Func<ICodeElement, ICodeElement, T> make;

        public BinaryPopulateScope(IPopulateScope<ICodeElement> left, IPopulateScope<ICodeElement> right, Func<ICodeElement, ICodeElement, T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReferance<T> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this);
            return new BinaryResolveReferance<T>(left.Run(nextContext), right.Run(nextContext), make);
        }
    }



    public class BinaryResolveReferance<T> : IResolveReferance<T> 
        where T: ICodeElement
    {
        public readonly IResolveReferance<ICodeElement> left;
        public readonly IResolveReferance<ICodeElement> right;
        private readonly Func<ICodeElement, ICodeElement, T> make;
        private readonly FollowBox<ITypeDefinition> followBox = new FollowBox<ITypeDefinition>();

        public BinaryResolveReferance(IResolveReferance<ICodeElement> resolveReferance1, IResolveReferance<ICodeElement> resolveReferance2, Func<ICodeElement, ICodeElement, T> make)
        {
            this.left = resolveReferance1;
            this.right = resolveReferance2;
            this.make = make;
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return followBox;
        }

        public T Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this);
            var res = make(left.Run(nextContext), right.Run(nextContext));
            followBox.Follow(res.ReturnType(context.Tree));
            return res;
        }
    }

}
