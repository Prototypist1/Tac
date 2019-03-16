using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public class  InternalAssemblyBacking : IBacking
        {
            private readonly Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>> memberValues;

            internal InternalAssemblyBacking(Dictionary<IKey, IInterpetedOperation<IInterpetedAnyType>> memberValues)
            {
                this.memberValues = memberValues ?? throw new ArgumentNullException(nameof(memberValues));
            }

            // TODO someday this will comsume an object with the right members to defined and initialize them!

        }

        public IAssembly<InternalAssemblyBacking> Build() {
            var scope = new Scope();

            scope.Build(
                members, 
                new List<TypeData>() { },
                new List<GenericTypeData>() { });

            return new Assembly(
                key,
                scope,
                new InternalAssemblyBacking(memberValues)
                );
        }

        private class Assembly : IAssembly<InternalAssemblyBacking>
        {
            public Assembly(IKey key, IFinalizedScope scope, InternalAssemblyBacking backing)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.Backing = backing ?? throw new ArgumentNullException(nameof(backing));
            }

            public IReadOnlyList<IAssembly> Referances => new List<IAssembly>();

            public IKey Key { get; }
            public IFinalizedScope Scope { get; }
            public InternalAssemblyBacking Backing {get;}

            public T Convert<T, TBaking>(IOpenBoxesContext<T,TBaking> context)
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
