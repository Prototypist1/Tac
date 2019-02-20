using Prototypist.LeftToRight;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedMethod<TIn, TOut> : IInterpetedCallable<TIn, TOut>
    {
    }

    public interface IInterpetedCallable<TIn, TOut> : IInterpetedData
    {
        IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input);
    }

    internal class InterpetedMethod<TIn, TOut> : IInterpetedMethod<TIn, TOut>
    {
        public InterpetedMethod(
            InterpetedMemberDefinition<TIn> parameterDefinition,
            IInterpetedOperation<object>[] body, 
            InterpetedContext context,
            IInterpetedScopeTemplate scope) 
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
        }

        private InterpetedMemberDefinition<TIn> ParameterDefinition { get; }
        private IInterpetedOperation<object>[] Body { get; }
        private InterpetedContext Context { get; }
        private IInterpetedScopeTemplate Scope { get; }
        private InterpetedStaticScope StaticScope { get; } = InterpetedStaticScope.Empty();
        
        public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input) {

            var res = Scope.Create();


            if (!res.TryAddMember(ParameterDefinition.Key, input)) {
                throw new System.Exception("trash, it is all trash!");
            }

            var scope = Context.Child(res);

            foreach (var line in Body)
            {
                var result =  line.Interpet(scope);
                if (result.IsReturn(out var resMember, out var value))
                {
                    return InterpetedResult.Create(resMember.Cast<IInterpetedMember<TOut>>());
                }
            }

            if (typeof(IInterpedEmpty).IsAssignableFrom(typeof(TOut))){
                dynamic hack = new RunTimeEmpty();
                return InterpetedResult.Create<IInterpetedMember<TOut>>(hack);
            }

            throw new System.Exception("method did not return!");

            
        }
    }
}