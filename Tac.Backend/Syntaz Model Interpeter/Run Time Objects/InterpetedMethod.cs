using Prototypist.LeftToRight;
using System;

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


    public static partial class TypeManager
    {

        internal static IInterpetedMethod<TIn, TOut> InternalMethod<TIn, TOut>(InterpetedMemberDefinition<TIn> parameterDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { InterpetedMethodIntention<TIn, TOut>(parameterDefinition, body, context, scope) }).Has<IInterpetedMethod<TIn, TOut>>();

        internal static Func<IRunTimeAnyRoot, IInterpetedMethod<TIn, TOut>> InterpetedMethodIntention<TIn, TOut>(
                InterpetedMemberDefinition<TIn> parameterDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType 
            => root => new InterpetedMethod<TIn, TOut>(parameterDefinition, body, context, scope, root);


        private class InterpetedMethod<TIn, TOut> : RootedTypeAny, IInterpetedMethod<TIn, TOut>
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
        {
            public InterpetedMethod(
                InterpetedMemberDefinition<TIn> parameterDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IRunTimeAnyRoot root) : base(root)
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
            private IInterpetedStaticScope StaticScope { get; } = TypeManager.EmptyStaticScope();

            public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input)
            {

                var res = Scope.Create();

                res.GetMember<TIn>(ParameterDefinition.Key).Cast<IInterpetedMemberSet<TIn>>().Set(input.Value);

                var scope = Context.Child(res);

                foreach (var line in Body)
                {
                    var result = line.Interpet(scope);
                    if (result.IsReturn(out var resMember, out var value))
                    {
                        return InterpetedResult.Create(resMember.Cast<IInterpetedMember<TOut>>());
                    }
                }

                if (typeof(IInterpedEmpty).IsAssignableFrom(typeof(TOut)))
                {
                    var hack = TypeManager.Empty();
                    return InterpetedResult.Create(Member(hack.Cast<TOut>()));
                }

                throw new System.Exception("method did not return!");


            }
        }
    }
}