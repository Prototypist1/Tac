using Prototypist.LeftToRight;
using Tac.Model;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedMethod<TIn, TOut> : IInterpetedData
        where TIn : Run_Time_Objects.IInterpeted
        where TOut : Run_Time_Objects.IInterpeted
    {
        IInterpetedResult<TOut> Invoke(Run_Time_Objects.IInterpeted input);
    }

    internal class InterpetedMethod: IInterpetedMethod
    {
        public InterpetedMethod(
            InterpetedMemberDefinition parameterDefinition,
            IInterpeted[] body, 
            InterpetedContext context,
            IInterpetedScopeTemplate scope) 
        {
            ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
            Context = context ?? throw new System.ArgumentNullException(nameof(context));
            Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
        }

        private InterpetedMemberDefinition ParameterDefinition { get; }
        private IInterpeted[] Body { get; }
        private InterpetedContext Context { get; }
        private IInterpetedScopeTemplate Scope { get; }
        private InterpetedStaticScope StaticScope { get; } = InterpetedStaticScope.Empty();
        
        public InterpetedResult Invoke(Run_Time_Objects.IInterpeted input) {

            var res = Scope.Create();

            res.GetMember(ParameterDefinition.Key).Value = input;

            var scope = Context.Child(res);

            foreach (var line in Body)
            {
                var result =  line.Cast<IInterpetedOperation>().Interpet(scope);
                if (result.IsReturn) {
                    return result;
                }
            }

            return InterpetedResult.Create();
        }
    }
}