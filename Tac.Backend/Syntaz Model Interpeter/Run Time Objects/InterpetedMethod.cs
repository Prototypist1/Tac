using Prototypist.LeftToRight;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedMethod<in TIn,  out TOut> : IInterpetedCallable<TIn, TOut>
        where TOut : IInterpetedAnyType
        where TIn : IInterpetedAnyType
    {
    }

    public interface IInterpetedCallable<in TIn, out TOut> : IInterpetedAnyType
        where TOut : IInterpetedAnyType
        where TIn : IInterpetedAnyType
    {
        IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input);
    }

    internal class InterpetedMethod<TIn, TOut> : RunTimeAny, IInterpetedMethod<TIn, TOut>
        where TIn :  IInterpetedAnyType
        where TOut :  IInterpetedAnyType
    {
        public InterpetedMethod(
            InterpetedMemberDefinition<TIn> parameterDefinition,
            IInterpetedOperation<IInterpetedAnyType>[] body, 
            InterpetedContext context,
            IInterpetedScopeTemplate scope) 
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
        }

        private InterpetedMemberDefinition<TIn> ParameterDefinition { get; }
        private IInterpetedOperation<IInterpetedAnyType>[] Body { get; }
        private InterpetedContext Context { get; }
        private IInterpetedScopeTemplate Scope { get; }
        private InterpetedStaticScope StaticScope { get; } = InterpetedStaticScope.Empty();
        
        public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input) {

            var res = Scope.Create();

            res.GetMember<TIn>(ParameterDefinition.Key).Cast<IInterpetedMemberSet<TIn>>().Set(input.Value);

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
                var hack = new RunTimeEmpty();
                return InterpetedResult.Create<IInterpetedMember<TOut>>(new InterpetedMember<TOut>( hack.Cast<TOut>()));
            }

            throw new System.Exception("method did not return!");

            
        }
    }
}