using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.New
{
    public interface IMaker<T>
        where T : ICodeElement
    {
        bool TryMake(ElementToken elementToken, ElementMatchingContext matchingContext, out IPopulateScope<T> result);
    }

    public interface IOperationMaker<T>
    where T : ICodeElement
    {
        bool TryMake(IEnumerable<IToken> elementToken, ElementMatchingContext matchingContext, out IPopulateScope<T> result);
    }

    public interface IPipelineContext<T,TSelf>
        where TSelf: IPipelineContext<T, TSelf>
    {
        ScopeTree Tree { get; }
        bool TryGetParentContext(out TSelf parent);
        bool TryGetParent<TT>(out TT parent) where TT :T;
        TSelf Child(T step);
        TSelf Child(T step, IScope scope);
    }

    // hmm the parsing is almost a step as well? 

    public interface IPopulateScopeContext: IPipelineContext<IPopulateScope, IPopulateScopeContext> {}
    
    public interface IResolveReferanceContext : IPipelineContext<IResolveReferance, IResolveReferanceContext> {}

    public interface IPopulateScope { }

    public interface IPopulateScope<out T> : IPopulateScope
    {
        IResolveReferance<T> Run(IPopulateScopeContext context);
    }

    public interface IResolveReferance { }

    public interface IResolveReferance<out T> : IResolveReferance
    {
        T Run(IResolveReferanceContext context);
    }
    
}
