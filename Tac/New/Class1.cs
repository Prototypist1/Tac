using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Model.Elements;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;


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

    internal interface IMaker
    {
        ITokenMatching TryMake(ITokenMatching elementToken);
    }

    internal interface IMaker<out TCodeElement>
    {
        ITokenMatching<TCodeElement> TryMake(IMatchedTokenMatching elementToken);
    }
    


    // hmm the parsing is almost a step as well? 

    internal interface IPopulateScopeContext {

        IPopulatableScope Scope { get; }
        IPopulateScopeContext Child();
        IPopulateScopeContext TemplateChild(Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] parameters);
        IResolvableScope GetResolvableScope();

    }

    internal class PopulateScopeContext : IPopulateScopeContext
    {
        private readonly NewScope stack;

        public PopulateScopeContext(NewScope stack)
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public IPopulatableScope Scope
        {
            get { return stack; }
        }
        

        public IPopulateScopeContext Child()
        {
            return new PopulateScopeContext(new NewScope(stack));
        }

        public IResolvableScope GetResolvableScope()
        {
            return stack.ToResolvable();
        }

        public IPopulateScopeContext TemplateChild(Tac._3_Syntax_Model.Elements.Atomic_Types.GemericTypeParameterPlacholder[] parameters)
        {
            // TODO.. 
            // what is this... why is this...
            // this project is too big for my brain!
            // I think this is a old way to do generics
            // and I should probably remove it 
            var template = new ScopeTemplate(parameters,stack);
            return new PopulateScopeContext(template);
        }
    }

    public interface IResolveReferenceContext  {
    }

    public class ResolveReferanceContext : IResolveReferenceContext
    {
    }

    // TODO I think I should protect these!
    // you are only allowed to put things in scope during this step

    internal interface IPopulateScope {
        IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType();
    }

    internal interface IPopulateScope<out TCodeElement> : IPopulateScope
    {
        IPopulateBoxes<TCodeElement> Run(IPopulateScopeContext context);
    }

    // TODO I think I should protect these!
    // you should only pull things out of scope during this step
    public interface IResolveReferance {
    }
    
    // I think scopes have phases of production
    //

    public interface IPopulateBoxes<out TCodeElement> : IResolveReferance
    {
        IIsPossibly<TCodeElement> Run(IResolveReferenceContext context);
    }
    
}
