using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedImplementation<in  TIn,in  TMethodIn, out TMethodOut> : IInterpetedCallable<TIn, IInterpetedMethod<TMethodIn,TMethodOut>>
        where TIn : class, IInterpetedAnyType
        where TMethodIn : class, IInterpetedAnyType
        where TMethodOut : class, IInterpetedAnyType
    {
    }


    public static partial class TypeManager
    {
        internal static Func<IRunTimeAnyRoot, IInterpetedImplementation<TIn, TMethodIn, TMethodOut>> InterpetedImplementationIntention<TIn, TMethodIn, TMethodOut>(
                InterpetedMemberDefinition<TMethodIn> parameterDefinition,
                InterpetedMemberDefinition<TIn> contextDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope)
                   where TIn : class, IInterpetedAnyType
                    where TMethodIn : class, IInterpetedAnyType
                    where TMethodOut : class, IInterpetedAnyType
            => root => new InterpetedImplementation<TIn, TMethodIn, TMethodOut>(parameterDefinition, contextDefinition, body, context, scope, root);


        internal static IInterpetedImplementation<TIn, TMethodIn, TMethodOut> Implementation<TIn, TMethodIn, TMethodOut>(InterpetedMemberDefinition<TMethodIn> parameterDefinition,
                InterpetedMemberDefinition<TIn> contextDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope)
                   where TIn : class, IInterpetedAnyType
                    where TMethodIn : class, IInterpetedAnyType
                    where TMethodOut : class, IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { InterpetedImplementationIntention<TIn, TMethodIn, TMethodOut>(parameterDefinition, contextDefinition, body, context, scope) }).Has<IInterpetedImplementation<TIn, TMethodIn, TMethodOut>>();


        private class InterpetedImplementation<TIn, TMethodIn, TMethodOut> : RootedTypeAny, IInterpetedImplementation<TIn, TMethodIn, TMethodOut>
        where TIn : class, IInterpetedAnyType
        where TMethodIn : class, IInterpetedAnyType
        where TMethodOut : class, IInterpetedAnyType
        {

            public InterpetedImplementation(
                InterpetedMemberDefinition<TMethodIn> parameterDefinition,
                InterpetedMemberDefinition<TIn> contextDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope, 
                IRunTimeAnyRoot root) : base(root)
            {
                ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                Body = body ?? throw new ArgumentNullException(nameof(body));
                Context = context ?? throw new ArgumentNullException(nameof(context));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            }

            private readonly InterpetedMemberDefinition<TIn> contextDefinition;
            private InterpetedMemberDefinition<TMethodIn> ParameterDefinition { get; }
            private IInterpetedOperation<IInterpetedAnyType>[] Body { get; }
            private InterpetedContext Context { get; }
            private IInterpetedScopeTemplate Scope { get; }

            public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TMethodIn, TMethodOut>>> Invoke(IInterpetedMember<TIn> input)
            {

                var context = Context.Child(TypeManager.InstanceScope((contextDefinition.Key, input)));

                return 
                    InterpetedResult.Create(
                        Member(
                            InternalMethod<TMethodIn, TMethodOut>(
                                ParameterDefinition,
                                Body,
                                context,
                                Scope)));
            }
        }
    }
}