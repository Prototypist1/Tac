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

        // kind of a moot point really 
        public IBox<ITypeDefinition> ReturnType(IScope scope)
        {
            return scope.GetTypeOrThrow(RootScope.EmptyType);
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

        public IResult<IPopulateScope<T>> TryMake(IEnumerable<IToken> tokens, ElementMatchingContext matchingContext)
        {
            if (TokenMatching.Start(tokens)
            .Has(ElementMatcher.IsTrailingOperation(Name), out var perface, out var _)
            .IsMatch)
            {
                var left = matchingContext.ParseLine(perface);
                
                return ResultExtension.Good(new TrailingPopulateScope<T>(left,Make));
            }
            return ResultExtension.Bad<IPopulateScope<T>>();
        }
        
    }

    public class TrailingPopulateScope<T> : IPopulateScope<T>
        where T : ICodeElement
    {
        private readonly IPopulateScope<ICodeElement> left;
        private readonly Func<ICodeElement, T> make;

        public TrailingPopulateScope(IPopulateScope<ICodeElement> left, Func<ICodeElement, T> make)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.make = make ?? throw new ArgumentNullException(nameof(make));
        }

        public IResolveReference<T> Run(IPopulateScopeContext context)
        {
            var nextContext = context.Child(this);
            return new TrailingResolveReferance<T>(left.Run(nextContext),  make);
        }
    }



    public class TrailingResolveReferance<T> : IResolveReference<T>
        where T : ICodeElement
    {
        public readonly IResolveReference<ICodeElement> left;
        private readonly Func<ICodeElement, T> make;
        private readonly FollowBox<ITypeDefinition> followBox = new FollowBox<ITypeDefinition>();

        public TrailingResolveReferance(IResolveReference<ICodeElement> resolveReferance1,  Func<ICodeElement, T> make)
        {
            left = resolveReferance1;
            this.make = make;
        }

        public IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context)
        {
            return followBox;
        }

        public T Run(IResolveReferanceContext context)
        {
            var nextContext = context.Child(this);
            var res = make(left.Run(nextContext));
            followBox.Follow(res.ReturnType(context.Tree.root));
            return res;
        }
    }


    public class ReturnOperationMaker : TrailingOperationMaker<ReturnOperation>
    {
        public ReturnOperationMaker(Func<ICodeElement, ReturnOperation> make) : base("return", make)
        {
        }
    }
}
