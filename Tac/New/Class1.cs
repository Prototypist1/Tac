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

    public interface IPopulateScope<out T>
    {
        IResolveReferance<T> Run(ScopeTree tree);
    }
    
    public interface IResolveReferance<out T> {
        T Run(ScopeTree tree);
    }
    
}
