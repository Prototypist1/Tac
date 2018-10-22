using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using static Tac.Semantic_Model.ScopeTree;

namespace Tac.New
{
    public static class ResultExtension{
        public static bool TryGetValue<T>(this IResult<T> self, out T res) {
            if (self.HasValue) {
                res = self.Value;
                return true;

            }
            res = default;
            return false;
        }
        
        public static Result<T> Good<T>(T value)
        {
            return new Result<T>(true, value);
        }

        public static Result<T> Bad<T>()
        {
            return new Result<T>(false, default);
        }
    }
    
    public interface IResult<out T>
    {
        bool HasValue { get; }
        T Value { get; }
    }
    
    public class Result<T> : IResult<T>
    {
        public Result(bool hasResult, T value)
        {
            HasValue = hasResult;
            Value = value;
        }

        public bool HasValue { get;}
        public T Value {get;}

    }
    
    public interface IMaker<out T>
        where T : ICodeElement
    {
        IResult<IPopulateScope<T>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext);
    }

    public interface IOperationMaker<out T>
    where T : ICodeElement
    {
        IResult<IPopulateScope<T>> TryMake(IEnumerable<IToken> elementToken, ElementMatchingContext matchingContext);
    }

    public interface IPipelineContext
    {
        IElementBuilders ElementBuilders { get; }
    }

    // hmm the parsing is almost a step as well? 

    public interface IPopulateScopeContext: IPipelineContext {

        IPopulatableScope Scope { get; }
        IPopulateScopeContext Child();
        IResolvableScope GetResolvableScope();

    }

    public class PopulateScopeContext : IPopulateScopeContext
    {
        private readonly ScopeStack stack;

        public PopulateScopeContext(ScopeStack stack, IPopulatableScope scope, IElementBuilders elementBuilders)
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        public IPopulatableScope Scope
        {
            get;
        }

        public IElementBuilders ElementBuilders
        {
            get;
        }

        public IPopulateScopeContext Child()
        {
            var (child, nextStack) = stack.ChildScope();
            return new PopulateScopeContext(nextStack, child, ElementBuilders);
        }

        public IResolvableScope GetResolvableScope()
        {
            return stack.ToResolvable();
        }
    }

    public interface IResolveReferanceContext : IPipelineContext {
    }

    public class ResolveReferanceContext : IResolveReferanceContext
    {
        public ResolveReferanceContext(IElementBuilders elementBuilders)
        {
            ElementBuilders = elementBuilders ?? throw new ArgumentNullException(nameof(elementBuilders));
        }

        public IElementBuilders ElementBuilders
        {
            get;
        }
    }

    // TODO I think I should protect these!
    // you are only allowed to put things in scope during this step

    public interface IPopulateScope {
        IBox<IReturnable> GetReturnType(IElementBuilders elementBuilders);
    }

    public interface IPopulateScope<out T> : IPopulateScope
    {
        IResolveReference<T> Run(IPopulateScopeContext context);
    }

    // TODO I think I should protect these!
    // you should only pull things out of scope during this step
    public interface IResolveReferance {
    }
    
    // I think scopes have phases of production
    //

    public interface IResolveReference<out T> : IResolveReferance
    {
        T Run(IResolveReferanceContext context);
    }
}
