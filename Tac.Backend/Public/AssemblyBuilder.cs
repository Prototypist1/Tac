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
        private readonly List<IInterpetedOperation<IInterpetedAnyType>> init = new List<IInterpetedOperation<IInterpetedAnyType>>();
        private readonly List<(IInterpetedMemberDefinition<IInterpetedAnyType>, ITypeReferance)> members = new List<(IInterpetedMemberDefinition<IInterpetedAnyType>, ITypeReferance)>();

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
            var assign = new InterpetedAssignOperation<IInterpetedMethod<TIn,TOut>>();
            var method = new InterpetedExternalMethodDefinition<TIn,TOut>();
            method.Init(func);
            var memberReference = new InterpetedMemberReferance<IInterpetedMethod<TIn, TOut>>();
            memberReference.Init(memberDef);
            assign.Init(method, memberReference);
            init.Add(assign);
            members.Add((memberDef,typeReferance));
            return this;
        }

        public IAssembly Build() {
            var scope = new Scope();

            scope.Build(
                members.Select(x =>  new IsStatic(Model.Instantiated.MemberDefinition.CreateAndBuild(x.Item1.Key,x.Item2, true), true)).ToList(), 
                new List<TypeData>() { },
                new List<GenericTypeData>() { });

            return new Assembly(
                key,
                scope,
                init);
        }

        private class MemberDefinition : IMemberDefinition
        {
            public MemberDefinition(IKey key, ITypeReferance type, bool readOnly)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Type = type ?? throw new ArgumentNullException(nameof(type));
                ReadOnly = readOnly;
            }

            public IKey Key {get;}
            public ITypeReferance Type {get;}
            public bool ReadOnly {get;}
            public T Convert<T>(IOpenBoxesContext<T> context)=>context.MemberDefinition(this);
            public IVerifiableType Returns() => Type.TypeDefinition;
        }

        private class Assembly : IAssembly
        {
            private IReadOnlyList<IInterpetedOperation<IInterpetedAnyType>> initialization;
            public Assembly(IKey key, IFinalizedScope scope, IReadOnlyList<IInterpetedOperation<IInterpetedAnyType>> initialization)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.initialization = initialization ?? throw new ArgumentNullException(nameof(initialization));
            }

            public IReadOnlyList<IAssembly> Referances => new List<IAssembly>();

            public IKey Key { get; }
            public IFinalizedScope Scope { get; }

            public T Convert<T>(IOpenBoxesContext<T> context)
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
