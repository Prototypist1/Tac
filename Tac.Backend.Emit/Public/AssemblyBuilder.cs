using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Emit.SyntaxModel;
using Tac.Backend.Emit.SyntaxModel.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Emit.SyntaxModel;
using static Tac.Model.Instantiated.Scope;
using Prototypist.Toolbox.Object;

namespace Tac.Backend.Emit.Public
{



    public class AssemblyBuilder
    {
        private readonly IKey key;
        private readonly Dictionary<IKey, (IAssembledOperation,IVerifiableType)> memberValues = new Dictionary<IKey, (IAssembledOperation, IVerifiableType)>();
        private readonly List<IsStatic> members = new List<IsStatic>();

        public AssemblyBuilder(IKey key)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AssemblyBuilder AddMethod(IKey key, Func<IInterpetedAnyType, IInterpetedAnyType> func, IMethodType type)
        {
            var memberDef = new InterpetedMemberDefinition();
            memberDef.Init(key, type);
            var method = new InterpetedExternalMethodDefinition();
            method.Init(func, type);
            memberValues.Add(key, (method,type));
            members.Add(new IsStatic(MemberDefinition.CreateAndBuild(key, type, true),false));
            return this;
        }

        public class  InterpetedAssemblyBacking : IBacking
        {
            private readonly Dictionary<IKey, (IAssembledOperation, IVerifiableType)> memberValues;
            private readonly IFinalizedScope scope;

            internal InterpetedAssemblyBacking(Dictionary<IKey, (IAssembledOperation, IVerifiableType)> memberValues, IFinalizedScope scope)
            {
                this.memberValues = memberValues ?? throw new ArgumentNullException(nameof(memberValues));
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            }

            internal IInterpetedMember CreateMember(AssemblyContext interpetedContext)
            {
                var scopeTemplate = TypeManager.InstanceScope(scope, scope.ToVerifiableType());
                var objectDefinition = new InterpetedObjectDefinition();
                objectDefinition.Init(scopeTemplate, memberValues.Select(memberValuePair => {
                    //var typeParameter = memberValuePair.Value.Item1.GetType().GetInterfaces().Where(x=>x.GetGenericTypeDefinition().Equals(typeof(IInterpetedOperation<>))).Single().GetGenericArguments().First();
                    //    var method = typeof(InterpetedAssemblyBacking).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x =>
                    //      x.Name == nameof(GetAssignemnt) && x.IsGenericMethod);
                    //    var madeMethod= method.MakeGenericMethod(new[] { typeParameter , typeParameter });
                    //return madeMethod.Invoke(this, new object[] { memberValuePair.Key, memberValuePair.Value.Item1, memberValuePair.Value.Item2 }).CastTo<IInterpetedAssignOperation>();

                    return GetAssignemnt(memberValuePair.Key, memberValuePair.Value.Item1, memberValuePair.Value.Item2).CastTo<IInterpetedAssignOperation>();
                }));

                if (objectDefinition.Assemble(interpetedContext).IsReturn(out var _, out var value)) {
                    throw new Exception("this should not throw");
                }

                return value!;
            }

            private IInterpetedAssignOperation GetAssignemnt(IKey key, IAssembledOperation operation, IVerifiableType type)
            {
                var memberDefinition = new InterpetedMemberDefinition();
                memberDefinition.Init(key, type);

                var memberReference = new InterpetedMemberReferance();
                memberReference.Init(memberDefinition);
                var assign = new InterpetedAssignOperation();
                assign.Init(operation, memberReference);
                return assign;
            }


        }

        public IAssembly<InterpetedAssemblyBacking> Build() {
            var scope = new Scope();

            scope.Build(members);

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
