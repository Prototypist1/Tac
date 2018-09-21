using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.New
{
    public interface IMaker<T>
        where T: ICodeElement
    {
        bool TryMake(ElementToken elementToken , ElementMatchingContext matchingContext, out Steps.PopulateScope<T> result);
    }

    public static class Steps
    {
        public delegate DetermineInferedTypes<T> PopulateScope<out T>();
        public delegate ResolveReferance<T> DetermineInferedTypes<out T>();
        public delegate T ResolveReferance<out T>();
    }
}
