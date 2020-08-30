using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using Tac.Model.Elements;
using Tac.Model.Instantiated;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IInterpetedMethod : IInterpetedAnyType { }

    public interface IInterpetedMethod<in TIn, out TOut>: IInterpetedMethod
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
                IInterpetedScopeTemplate scope,
                IMethodType methodType)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
            => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { InterpetedMethodIntention<TIn, TOut>(parameterDefinition, body, context, scope, methodType) }).Has<IInterpetedMethod<TIn, TOut>>();

        internal static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> InterpetedMethodIntention<TIn, TOut>(
                InterpetedMemberDefinition<TIn> parameterDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
                InterpetedContext context,
                IInterpetedScopeTemplate scope,
                IMethodType methodType)
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
            => root => {
                var item = new InterpetedMethod<TIn, TOut>(parameterDefinition, body, context, scope, methodType,root);
                return new RunTimeAnyRootEntry(item, methodType);
            };


        private class InterpetedMethod<TIn, TOut> : RootedTypeAny, IInterpetedMethod<TIn, TOut>
            where TIn : IInterpetedAnyType
            where TOut : IInterpetedAnyType
        {
            public InterpetedMethod(
                InterpetedMemberDefinition<TIn> parameterDefinition,
                IInterpetedOperation<IInterpetedAnyType>[] body,
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

            private InterpetedMemberDefinition<TIn> ParameterDefinition { get; }
            private IInterpetedOperation<IInterpetedAnyType>[] Body { get; }
            private InterpetedContext Context { get; }
            private IInterpetedScopeTemplate Scope { get; }
            public IMethodType MethodType { get; }
            private IInterpetedStaticScope StaticScope { get; } = TypeManager.EmptyStaticScope();

            public IInterpetedResult<IInterpetedMember<TOut>> Invoke(IInterpetedMember<TIn> input)
            {

                var res = Scope.Create();

                res.GetMember<TIn>(ParameterDefinition.Key).CastTo<IInterpetedMemberSet<TIn>>().Set(input.Value);

                var scope = Context.Child(res);

                foreach (var line in Body)
                {
                    var result = line.Interpet(scope);
                    if (result.IsReturn(out var resMember, out var value))
                    {
                        return InterpetedResult.Create(resMember.CastTo<IInterpetedMember<TOut>>());
                    }
                }

                // does this work??
                // wierd stuff around the way I am doing types
                // life would be so much simpler if I just pulled it all out
                // I should just pull it all out
                // clearly I should
                // 
                if (typeof(IInterpedEmpty).IsAssignableFrom(typeof(TOut)))
                {
                    var hack = TypeManager.Empty();
                    return InterpetedResult.Create(Member<TOut>(hack.Convert(TransformerExtensions.NewConversionContext()), hack.CastTo<TOut>()));
                }

                throw new System.Exception("method did not return!");


            }
        }
    }
}