using System;
using System.Collections.Generic;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ReturnOperation : ICodeElement
    {
        public ReturnOperation(ICodeElement result)
        {
            Result = result;
        }

        public ICodeElement Result { get; }

        public ITypeDefinition ReturnType(ScopeStack scope)
        {
            return scope.GetType(RootScope.EmptyType);
        }
    }
    
    public class TrailingOperationMaker<T> : IOperationMaker<T>
        where T : class, ICodeElement
    {
        public TrailingOperationMaker(string name, Func<ICodeElement, T> make)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public string Name { get; }
        private Func<ICodeElement, T> Make { get; }

        public bool TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext, out Steps.PopulateScope<T> result)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsTrailingOperation(Name), out var perface, out var _)
            .IsMatch)
            {
                Steps.PopulateScope<ICodeElement> left = matchingContext.ParseLine(perface);

                result = PopulateScope(left);
                return true;
            }

            result = default;
            return false;
        }

        private Steps.PopulateScope<T> PopulateScope(Steps.PopulateScope<ICodeElement> left)
        {
            return (tree) =>
            {
                return DetermineInferedTypes(left(tree));
            };
        }

        private Steps.DetermineInferedTypes<T> DetermineInferedTypes(Steps.DetermineInferedTypes<ICodeElement> left)
        {
            return () =>
            {
                return ResolveReferance(left());
            };
        }

        private Steps.ResolveReferance<T> ResolveReferance(Steps.ResolveReferance<ICodeElement> left)
        {
            return (tree) =>
            {
                return Make(left(tree));
            };
        }
    }

    public class ReturnOperationMaker : TrailingOperationMaker<ReturnOperation>
    {
        public ReturnOperationMaker(Func<ICodeElement, ReturnOperation> make) : base("return", make)
        {
        }
    }
}
