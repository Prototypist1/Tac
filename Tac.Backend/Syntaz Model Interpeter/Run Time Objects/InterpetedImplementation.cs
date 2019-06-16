using Prototypist.LeftToRight;
using System;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Elements;

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
        internal static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InterpetedImplementationIntention<TIn, TMethodIn, TMethodOut>(
                InterpetedMemberDefinition<TMethodIn> parameterDefinition,
                InterpetedMemberDefinition<TIn> contextDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IImplementationType implementationType)
                   where TIn : class, IInterpetedAnyType
                    where TMethodIn : class, IInterpetedAnyType
                    where TMethodOut : class, IInterpetedAnyType
            => root =>
            {

                var item = new InterpetedImplementation<TIn, TMethodIn, TMethodOut>(parameterDefinition, contextDefinition, body, context, scope, implementationType, root);
                return new RunTimeAnyRootEntry(item, implementationType);
            };


        internal static IInterpetedImplementation<TIn, TMethodIn, TMethodOut> Implementation<TIn, TMethodIn, TMethodOut>(InterpetedMemberDefinition<TMethodIn> parameterDefinition,
                InterpetedMemberDefinition<TIn> contextDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IImplementationType implementationType)
                   where TIn : class, IInterpetedAnyType
                    where TMethodIn : class, IInterpetedAnyType
                    where TMethodOut : class, IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InterpetedImplementationIntention<TIn, TMethodIn, TMethodOut>(parameterDefinition, contextDefinition, body, context, scope, implementationType) }).Has<IInterpetedImplementation<TIn, TMethodIn, TMethodOut>>();


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
                IImplementationType implementationType,
                IRunTimeAnyRoot root) : base(root)
            {
                ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                Body = body ?? throw new ArgumentNullException(nameof(body));
                InterpetedContext = context ?? throw new ArgumentNullException(nameof(context));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            }

            private readonly InterpetedMemberDefinition<TIn> contextDefinition;
            private InterpetedMemberDefinition<TMethodIn> ParameterDefinition { get; }
            private IInterpetedOperation<IInterpetedAnyType>[] Body { get; }
            private InterpetedContext InterpetedContext { get; }
            private IInterpetedScopeTemplate Scope { get; }
            public IImplementationType ImplementationType { get; }

            public IInterpetedResult<IInterpetedMember<IInterpetedMethod<TMethodIn, TMethodOut>>> Invoke(IInterpetedMember<TIn> input)
            {

                var context = InterpetedContext.Child(TypeManager.InstanceScope((contextDefinition.Key, input)));
                var thing = InternalMethod<TMethodIn, TMethodOut>(
                                ParameterDefinition,
                                Body,
                                context,
                                Scope,
                                MethodType.CreateAndBuild( 
                                    ImplementationType.InputType,
                                    ImplementationType.OutputType));
                return 
                    InterpetedResult.Create(
                        Member(
                            thing.Convert(TransformerExtensions.NewConversionContext()), 
                            thing));
            }
        }
    }
}