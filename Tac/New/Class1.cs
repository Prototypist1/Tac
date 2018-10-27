using System;
using System.Collections.Generic;
using System.Text;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;
using Tac.Semantic_Model.Operations;
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
    
    public interface IMaker<out TCodeElement>
    {
        IResult<IPopulateScope<TCodeElement>> TryMake(ElementToken elementToken, ElementMatchingContext matchingContext);
    }

    public interface IOperationMaker<out TCodeElement>
    {
        IResult<IPopulateScope<TCodeElement>> TryMake(IEnumerable<IToken> elementToken, ElementMatchingContext matchingContext);
    }
    

    // hmm the parsing is almost a step as well? 

    public interface IPopulateScopeContext {

        IPopulatableScope Scope { get; }
        IPopulateScopeContext Child();
        IResolvableScope GetResolvableScope();

    }

    // this is a very interesting patter
    // it is a manipulation of the type system
    // so that it enforces a contraint
    // the backend need to convert the model objects to it's own very of the model objects
    // it does so with this interface

    // it competes with the visister pattern
    // but, visiters have some down sides
    // if you add a visitable class
    // you have to remember to update a bunch of stuff
    // becuase the visitable classes do not generally know they are being visited
    // here the visited classes know they are being visited
    // and so it requires an entry here
    // which forces the front end to up
    public interface IOpenBoxesContext<T> 
    {
        T BlockDefinition(WeakBlockDefinition codeElement);
        T AssignOperation(WeakAssignOperation co);
        T ConstantNumber(WeakConstantNumber codeElement);
        T PathOperation(WeakPathOperation co);
        T GenericTypeDefinition(WeakGenericTypeDefinition codeElement);
        T ImplementationDefinition(WeakImplementationDefinition codeElement);
        T MemberDefinition(WeakMemberDefinition codeElement);
        T MemberReferance(WeakMemberReferance codeElement);
        T MethodDefinition(WeakMethodDefinition codeElement);
        T ModuleDefinition(WeakModuleDefinition codeElement);
        T LastCallOperation(WeakLastCallOperation co);
        T ObjectDefinition(WeakObjectDefinition codeElement);
        T TypeDefinition(WeakTypeDefinition codeElement);
        T AddOperation(WeakAddOperation co);
        T NextCallOperation(WeakNextCallOperation co);
        T ElseOperation(WeakElseOperation co);
        T IfTrueOperation(WeakIfTrueOperation co);
        T LessThanOperation(WeakLessThanOperation co);
        T MultiplyOperation(WeakMultiplyOperation co);
        T SubtractOperation(WeakSubtractOperation co);
        T ReturnOperation(WeakReturnOperation co);
    }

    public class PopulateScopeContext : IPopulateScopeContext
    {
        private readonly ScopeStack stack;

        public PopulateScopeContext(ScopeStack stack)
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        public IPopulatableScope Scope
        {
            get { return stack; }
        }
        

        public IPopulateScopeContext Child()
        {
            return new PopulateScopeContext(stack.ChildScope());
        }

        public IResolvableScope GetResolvableScope()
        {
            return stack.ToResolvable();
        }
    }

    public interface IResolveReferanceContext  {
    }

    public class ResolveReferanceContext : IResolveReferanceContext
    {
    }

    // TODO I think I should protect these!
    // you are only allowed to put things in scope during this step

    public interface IPopulateScope {
        IBox<IWeakReturnable> GetReturnType();
    }

    public interface IPopulateScope<out TCodeElement> : IPopulateScope
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
        IOpenBoxes<TCodeElement> Run(IResolveReferanceContext context);
    }

    public interface IOpenBoxes< out TCodeElement>
    { 
        TCodeElement CodeElement { get; }
        T Run<T>(IOpenBoxesContext<T> context);
    }
}
