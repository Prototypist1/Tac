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
        public BinaryOperationMaker(string name, Func<ICodeElement, ICodeElement, T> make,
            IElementBuilders elementBuilders)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        public string Name { get; }
        private Func<ICodeElement, ICodeElement, T> Make { get; }
        private IElementBuilders ElementBuilders { get; }

        public bool TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out IPopulateScope<T> result)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                IPopulateScope<ICodeElement> left = matchingContext.ParseLine(perface);
                IPopulateScope<ICodeElement> right = matchingContext.ParseParenthesisOrElement(rhs);

                result = new PopulateScope(left,right, Make);
                return true;
            }

            result = default;
            return false;
        }

        private class PopulateScope : IPopulateScope<T>
        {
            private readonly IPopulateScope<ICodeElement> left;
            private readonly IPopulateScope<ICodeElement> right;
            private readonly Func<ICodeElement, ICodeElement, T> make;

            public PopulateScope(IPopulateScope<ICodeElement> left, IPopulateScope<ICodeElement> right, Func<ICodeElement, ICodeElement, T> make)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.right = right ?? throw new ArgumentNullException(nameof(right));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IResolveReferance<T> Run(IPopulateScopeContext context)
            {
                var nextContext = context.Child(this);
                return new ResolveReferance(left.Run(nextContext), right.Run(nextContext), make);
            }

            public IResolveReferance<T> Run()
            {
                throw new NotImplementedException();
            }

            private class ResolveReferance : IResolveReferance<T>
            {
                private IResolveReferance<ICodeElement> resolveReferance1;
                private IResolveReferance<ICodeElement> resolveReferance2;
                private Func<ICodeElement, ICodeElement, T> make;

                public ResolveReferance(IResolveReferance<ICodeElement> resolveReferance1, IResolveReferance<ICodeElement> resolveReferance2, Func<ICodeElement, ICodeElement, T> make)
                {
                    this.resolveReferance1 = resolveReferance1;
                    this.resolveReferance2 = resolveReferance2;
                    this.make = make;
                }

                public T Run(IResolveReferanceContext context)
                {
                    var nextContext = context.Child(this);
                    return make(resolveReferance1.Run(nextContext), resolveReferance2.Run(nextContext));
                }
                
            }
        }
    }
}
