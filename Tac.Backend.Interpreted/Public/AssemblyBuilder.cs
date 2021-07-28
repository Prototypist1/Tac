using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using Tac.Backend.Interpreted.SyntazModelInterpeter.Elements;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Interpreted.SyntazModelInterpeter;
using static Tac.Model.Instantiated.Scope;
using Prototypist.Toolbox.Object;
using static Tac.Backend.Interpreted.Public.AssemblyBuilder;

namespace Tac.Backend.Interpreted.Public
{
    public class AssemblyBuilder: IAsseblyPendingType<IAssembly<InterpetedAssemblyBacking>, InterpetedAssemblyBacking>
    {
        private readonly NameKey key;
        private readonly Dictionary<IKey, (IInterpetedOperation,IVerifiableType)> memberValues = new Dictionary<IKey, (IInterpetedOperation, IVerifiableType)>();
        private readonly List<IsStatic> members = new List<IsStatic>();

        public NameKey Key => key;

        public IReadOnlyList<IMemberDefinition> Members => members.Select(x => x.Value).ToArray();

        public AssemblyBuilder(NameKey key)
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
            members.Add(new IsStatic(MemberDefinition.CreateAndBuild(key, type, Access.ReadOnly),false));
            return this;
        }

        public class  InterpetedAssemblyBacking 
        {
            private readonly Dictionary<IKey, (IInterpetedOperation, IVerifiableType)> memberValues;
            private readonly IFinalizedScope scope;

            internal InterpetedAssemblyBacking(Dictionary<IKey, (IInterpetedOperation, IVerifiableType)> memberValues, IFinalizedScope scope)
            {
                this.memberValues = memberValues ?? throw new ArgumentNullException(nameof(memberValues));
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            }

            internal IInterpetedMember CreateMember(InterpetedContext interpetedContext)
            {
                var scopeTemplate = new InterpetedScopeTemplate(scope, scope.ToVerifiableType());
                var objectDefinition = new InterpetedObjectDefinition();
                objectDefinition.Init(scopeTemplate, memberValues.Select(memberValuePair => {
                    //var typeParameter = memberValuePair.Value.Item1.GetType().GetInterfaces().Where(x=>x.GetGenericTypeDefinition().Equals(typeof(IInterpetedOperation<>))).Single().GetGenericArguments().First();
                    //    var method = typeof(InterpetedAssemblyBacking).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x =>
                    //      x.Name == nameof(GetAssignemnt) && x.IsGenericMethod);
                    //    var madeMethod= method.MakeGenericMethod(new[] { typeParameter , typeParameter });
                    //return madeMethod.Invoke(this, new object[] { memberValuePair.Key, memberValuePair.Value.Item1, memberValuePair.Value.Item2 }).CastTo<IInterpetedAssignOperation>();

                    return GetAssignemnt(memberValuePair.Key, memberValuePair.Value.Item1, memberValuePair.Value.Item2).CastTo<IInterpetedAssignOperation>();
                }));

                if (objectDefinition.Interpet(interpetedContext).IsReturn(out var _, out var value)) {
                    throw new Exception("this should not throw");
                }

                return value!;
            }

            private IInterpetedAssignOperation GetAssignemnt(IKey key, IInterpetedOperation operation, IVerifiableType type)
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

        public IAssembly<InterpetedAssemblyBacking> Convert(IInterfaceType interfaceType)
        {
            var scope = Scope.CreateAndBuild(memberValues.Select(x=> {

                var member = MemberDefinition.CreateAndBuild(x.Key, x.Value.Item2, Access.ReadOnly);

                return new IsStatic(member, false);
                }).ToList());

            return new Assembly(
                key,
                interfaceType,
                new InterpetedAssemblyBacking(memberValues, scope)
                );
        }

        private class Assembly : IAssembly<InterpetedAssemblyBacking>
        {
            public Assembly(NameKey key, IInterfaceType scope, InterpetedAssemblyBacking backing)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            }

            public IReadOnlyList<IAssembly<InterpetedAssemblyBacking>> References => new List<IAssembly<InterpetedAssemblyBacking>>();

            public NameKey Key { get; }
            public IInterfaceType Scope { get; }
            public InterpetedAssemblyBacking Backing {get;}
        }
    }
}
