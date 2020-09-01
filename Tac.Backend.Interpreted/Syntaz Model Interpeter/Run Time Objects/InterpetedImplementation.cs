using Prototypist.Toolbox;
using System;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;
using Tac.Model;
using Tac.Model.Instantiated;
using Tac.Model.Elements;
using Prototypist.Toolbox.Object;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedImplementation: IInterpetedMethod
    {
    }

    public static partial class TypeManager
    {
        internal static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InterpetedImplementationIntention(
                InterpetedMemberDefinition parameterDefinition,
                InterpetedMemberDefinition contextDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType implementationType)
            => root =>
            {

                var item = new InterpetedImplementation(parameterDefinition, contextDefinition, body, context, scope, implementationType, root);
                return new RunTimeAnyRootEntry(item, implementationType);
            };


        internal static IInterpetedImplementation Implementation(InterpetedMemberDefinition parameterDefinition,
                InterpetedMemberDefinition contextDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType implementationType)
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InterpetedImplementationIntention(parameterDefinition, contextDefinition, body, context, scope, implementationType) }).Has<IInterpetedImplementation>();


        private class InterpetedImplementation : RootedTypeAny, IInterpetedImplementation
        {


            public InterpetedImplementation(
                InterpetedMemberDefinition parameterDefinition,
                InterpetedMemberDefinition contextDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType implementationType,
                IRunTimeAnyRoot root) : base(root)
            {
                ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
                this.contextDefinition = contextDefinition ?? throw new ArgumentNullException(nameof(contextDefinition));
                Body = body ?? throw new ArgumentNullException(nameof(body));
                InterpetedContext = context ?? throw new ArgumentNullException(nameof(context));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            }

            private readonly InterpetedMemberDefinition contextDefinition;
            private InterpetedMemberDefinition ParameterDefinition { get; }
            private IInterpetedOperation[] Body { get; }
            private InterpetedContext InterpetedContext { get; }
            private IInterpetedScopeTemplate Scope { get; }
            public IMethodType ImplementationType { get; }

            public IInterpetedResult<IInterpetedMember> Invoke(IInterpetedMember input)
            {
                var context = InterpetedContext.Child(TypeManager.InstanceScope((contextDefinition.Key, input)));
                ImplementationType.OutputType.SafeIs(out MethodType outputMethod);
                var thing = InternalMethod(
                                ParameterDefinition,
                                Body,
                                context,
                                Scope,
                                Tac.Model.Instantiated.MethodType.CreateAndBuild(
                                    outputMethod.InputType,
                                    outputMethod.OutputType));
                return
                    InterpetedResult.Create(
                        Member(
                            thing.Convert(TransformerExtensions.NewConversionContext()),
                            thing));
            }
        }
    }
}