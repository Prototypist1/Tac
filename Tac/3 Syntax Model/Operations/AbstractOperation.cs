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

        public bool TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out Steps.PopulateScope<T> result)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsBinaryOperation(Name), out var perface, out var token, out var rhs)
            .IsMatch)
            {
                Steps.PopulateScope<ICodeElement> left = matchingContext.ParseLine(perface);
                Steps.PopulateScope<ICodeElement> right = matchingContext.ParseParenthesisOrElement(rhs);

                result = PopulateScope(left,right);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<T> PopulateScope(Steps.PopulateScope<ICodeElement> left, Steps.PopulateScope<ICodeElement> right)
        {
            return (tree) =>
            {
                return DetermineInferedTypes(left(tree), right(tree));
            };
        }

        private Steps.DetermineInferedTypes<T> DetermineInferedTypes(Steps.DetermineInferedTypes<ICodeElement> left, Steps.DetermineInferedTypes<ICodeElement> right)
        {
            return () =>
            {
                return ResolveReferance(left(), right());
            };
        }

        private Steps.ResolveReferance<T> ResolveReferance(Steps.ResolveReferance<ICodeElement> left, Steps.ResolveReferance<ICodeElement> right)
        {
            return (tree) =>
            {
                return Make(left(tree), right(tree));
            };
        }
    }
}
