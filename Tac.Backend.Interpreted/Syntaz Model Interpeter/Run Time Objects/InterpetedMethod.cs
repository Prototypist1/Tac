using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using Tac.Model.Elements;
using Tac.Model.Instantiated;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{

    public interface IInterpetedMethod : IInterpetedAnyType { 
        IInterpetedResult<IInterpetedMember> Invoke(IInterpetedMember input);
    }


    public static partial class TypeManager
    {

        internal static IInterpetedMethod InternalMethod(InterpetedMemberDefinition parameterDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType methodType)
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InterpetedMethodIntention(parameterDefinition, body, context, scope, methodType) }).Has<IInterpetedMethod>();

        internal static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InterpetedMethodIntention(
                InterpetedMemberDefinition parameterDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType methodType)
            => root => {
                var item = new InterpetedMethod(parameterDefinition, body, context, scope, methodType,root);
                return new RunTimeAnyRootEntry(item, methodType);
            };


        private class InterpetedMethod : RootedTypeAny, IInterpetedMethod
        {
            public InterpetedMethod(
                InterpetedMemberDefinition parameterDefinition,
                IInterpetedOperation[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType methodType,
                IRunTimeAnyRoot root) : base(root)
            {
                ParameterDefinition = parameterDefinition ?? throw new System.ArgumentNullException(nameof(parameterDefinition));
                Body = body ?? throw new System.ArgumentNullException(nameof(body));
                Context = context ?? throw new System.ArgumentNullException(nameof(context));
                Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
                MethodType = methodType ?? throw new ArgumentNullException(nameof(methodType));
            }

            private InterpetedMemberDefinition ParameterDefinition { get; }
            private IInterpetedOperation[] Body { get; }
            private InterpetedContext Context { get; }
            private IInterpetedScopeTemplate Scope { get; }
            public IMethodType MethodType { get; }
            private IInterpetedStaticScope StaticScope { get; } = TypeManager.EmptyStaticScope();

            public IInterpetedResult<IInterpetedMember> Invoke(IInterpetedMember input)
            {

                var res = Scope.Create();

                res.GetMember(ParameterDefinition.Key).CastTo<IInterpetedMemberSet>().Set(input.Value);

                var scope = Context.Child(res);

                foreach (var line in Body)
                {
                    var result = line.Interpet(scope);
                    if (result.IsReturn(out var resMember, out var value))
                    {
                        return InterpetedResult.Create(resMember.CastTo<IInterpetedMember>());
                    }
                }

                if (MethodType.OutputType is EmptyType) {
                    return InterpetedResult.Create(TypeManager.EmptyMember(TypeManager.Empty()));
                }

                throw new System.Exception("method did not return!");


            }
        }
    }
}