using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

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

    public interface IPipelineContext<T,TScope,TSelf>
        where TSelf: IPipelineContext<T, TScope, TSelf>
    {
        RootScope RootScope { get; }
        bool TryGetParentContext(out TSelf parent);
        bool TryGetParent<TT>(out TT parent) where TT :T;
        TSelf Child(T step);
        TSelf Child(T step, TScope scope);
    }

    // hmm the parsing is almost a step as well? 

    public interface IPopulateScopeContext: IPipelineContext<IPopulateScope, IPopulatableScope, IPopulateScopeContext> {
        bool TryGetMemberPath(NameKey name, out int depth, out IBox<MemberDefinition> box);
        bool TryAddMember(NameKey name, IBox<MemberDefinition> member);
        bool TryAddType(IKey name, IBox<ITypeDefinition> type);
    }
    
    public interface IResolveReferanceContext : IPipelineContext<IResolveReferance, IResolvableScope, IResolveReferanceContext> {
        IBox<MemberDefinition> GetMemberDefinition(NameKey key);
        IBox<ITypeDefinition> GetTypeDefintion(IKey key);
    }
    
    // TODO I think I should protect these!
    // you are only allowed to put things in scope during this step

    public interface IPopulateScope { }

    public interface IPopulateScope<out T> : IPopulateScope
    {
        IResolveReference<T> Run(IPopulateScopeContext context);
    }

    // TODO I think I should protect these!
    // you should only pull things out of scope during this step
    public interface IResolveReferance {
        IBox<ITypeDefinition> GetReturnType(IResolveReferanceContext context);
    }
    
    // I think scopes have phases of production
    //

    public interface IResolveReference<out T> : IResolveReferance
    {
        T Run(IResolveReferanceContext context);
    }
}
