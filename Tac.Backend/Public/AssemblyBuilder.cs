using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Syntaz_Model_Interpeter;
using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Syntaz_Model_Interpeter;
using static Tac.Model.Instantiated.Scope;

namespace Tac.Backend.Public
{



    public class AssemblyBuilder
    {
        private readonly IKey key;
        private readonly Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>> memberValues = new Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>>();
        private readonly List<IsStatic> members = new List<IsStatic>();

        public AssemblyBuilder(IKey key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AssemblyBuilder AddMethod<TIn,TOut>(IKey key, Func<TIn,TOut> func, ITypeReferance typeReferance)
            where TIn: IInterpetedAnyType
            where TOut : IInterpetedAnyType
        {
            var memberDef = new InterpetedMemberDefinition<IInterpetedMethod<TIn, TOut>>();
            memberDef.Init(key);
            var method = new InterpetedExternalMethodDefinition<TIn,TOut>();
            method.Init(func);
            memberValues.Add(key, method);
            members.Add(new IsStatic(MemberDefinition.CreateAndBuild(key, typeReferance, true),false));
            return this;
        }

        public class  InterpetedAssemblyBacking : IBacking
        {
            private readonly Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>> memberValues;
            private readonly IFinalizedScope scope;

            internal InterpetedAssemblyBacking(Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>> memberValues, IFinalizedScope scope)
            {
                this.memberValues = memberValues ?? throw new ArgumentNullException(nameof(memberValues));
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            }

            internal IInterpetedMember<IInterpetedAnyType> CreateMember(InterpetedContext interpetedContext)
            {
                var scopeTemplate = new InterpetedScopeTemplate(scope);
                var objectDefinition = new InterpetedObjectDefinition();
                objectDefinition.Init(scopeTemplate, memberValues.Select(memberValuePair => {
                    var typeParameter = memberValuePair.Value.GetType().GetInterfaces().Where(x=>x.GetGenericTypeDefinition().Equals(typeof(IInterpetedOperation<>))).Single().GetGenericArguments().First();
                        var method = typeof(InterpetedAssemblyBacking).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x =>
                          x.Name == nameof(GetAssignemnt) && x.IsGenericMethod);
                        var madeMethod= method.MakeGenericMethod(new[] { typeParameter , typeParameter });
                    return madeMethod.Invoke(this, new object[] { memberValuePair.Key, memberValuePair.Value }).Cast<IInterpetedAssignOperation<IInterpetedAnyType>>();
                }));

                var member = new InterpetedMember<IInterpetedScope>();
                if (objectDefinition.Interpet(interpetedContext).IsReturn(out var _, out var value)) {
                    throw new Exception("this should not throw");
                }
                member.Cast<IInterpetedMemberSet<IInterpetedScope>>().Set(value.Value);
                return member;
            }

            private IInterpetedAssignOperation<TLeft,TRight> GetAssignemnt<TLeft, TRight>(IKey key, IInterpetedOperation<TLeft> operation)
                where TLeft : TRight
                where TRight : IInterpetedAnyType
            {
                var memberDefinition = new InterpetedMemberDefinition<TRight>();
                memberDefinition.Init(key);

                var memberReference = new InterpetedMemberReferance<TRight>();
                memberReference.Init(memberDefinition);
                var assign = new InterpetedAssignOperation<TLeft,TRight>();
                assign.Init(operation, memberReference);
                return assign;
            }


        }

        public IAssembly<InterpetedAssemblyBacking> Build() {
            var scope = new Scope();

            scope.Build(
                members, 
                new List<TypeData>() { },
                new List<GenericTypeData>() { });

            return new Assembly(
                key,
                scope,
                new InterpetedAssemblyBacking(memberValues, scope)
                );
        }

        private class Assembly : IAssembly<InterpetedAssemblyBacking>
        {
            public Assembly(IKey key, IFinalizedScope scope, InterpetedAssemblyBacking backing)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            }

            public IReadOnlyList<IAssembly<InterpetedAssemblyBacking>> References => new List<IAssembly<InterpetedAssemblyBacking>>();

            public IKey Key { get; }
            public IFinalizedScope Scope { get; }
            public InterpetedAssemblyBacking Backing {get;}

            public T Convert<T, TBaking>(IOpenBoxesContext<T,TBaking> _)
                where TBaking:IBacking

            {
                throw new NotImplementedException();
            }

            public IVerifiableType Returns()
            {
                throw new NotImplementedException();
            }
        }

    }
}
