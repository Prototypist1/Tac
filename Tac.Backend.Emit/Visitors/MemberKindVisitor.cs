using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Emit.Lookup;
using Tac.Backend.Emit.Walkers;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;
using Tac.Type;

namespace Tac.Backend.Emit.Visitors
{

    class MemberKindVisitor : IOpenBoxesContext<Nothing>
    {

        private IReadOnlyList<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>> stack;
        private readonly MemberKindLookup lookup;
        private readonly WhoDefinedMemberByMethodlike extensionLookup;

        private MemberKindVisitor(IReadOnlyList<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>> stack, MemberKindLookup lookup, WhoDefinedMemberByMethodlike extensionLookup)
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this.lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
        }

        public static (MemberKindVisitor, MemberKindLookup) Make(WhoDefinedMemberByMethodlike extensionLookup, IProject<Assembly, object> project) {

            MemberKindLookup lookup = new MemberKindLookup();
            // 
            // TODO 
            // we can add project.DependencyScope directly to the lookups
            // this doesn't really need to happen in here, the MemberKindVisitor isn't involved at all
            // but it is a nessisray part of the flow
            //foreach (var member in project.DependencyScope.Members) {
            //    lookup.AddDependency(xxx, member);
            //}

            var res = new MemberKindVisitor(new List<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>>() { }, lookup, extensionLookup);
            //res.Init();
            return (res, lookup);
        }

        //private MemberKindVisitor Init() {
        //    foreach (var entry in rootScope.scope.Members.Values.Select(x => x.Value))
        //    {
        //        lookup.AddLocal(OrType.Make< IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, RootScope > (rootScope), entry);
        //    }
        //    return this;
        //}

        public MemberKindVisitor Push(ICodeElement another)
        {
            var list = stack.ToList();
            if (another.SafeIs(out IInternalMethodDefinition method))
            {
                list.Add( OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(method));
            }
            if (another.SafeIs(out IImplementationDefinition implementation))
            {
                list.Add( OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(implementation) );
            }
            if (another.SafeIs(out IEntryPointDefinition entry))
            {
                list.Add( OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(entry) );
            }
            if (another.SafeIs(out IRootScope root))
            {
                list.Add( OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(root) );
            }

            return new MemberKindVisitor(list, lookup, extensionLookup);
        }

        internal void HandleDependencies(IFinalizedScope dependencyScope)
        {
            foreach (var member in dependencyScope.Members.Values.Select(x=>x.Value))
            {
                lookup.AddDependency(member);
            }
        }

        private void Walk(IEnumerable<ICodeElement> codeElements) {
            foreach (var element in codeElements)
            {
                element.Convert(this);
            }
        }

        // operations:
        public Nothing AddOperation(IAddOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing AssignOperation(IAssignOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing PathOperation(IPathOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing ReturnOperation(IReturnOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing SubtractOperation(ISubtractOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            var owner = GetOwner();

            foreach (var entry in tryAssignOperation.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddLocal(owner, entry);
            }

            Push(tryAssignOperation).Walk(tryAssignOperation.Operands);

            return new Nothing();
        }

        private IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> GetOwner()
        {
            // climb the stack until you find a method or implementation or entry point
            return stack.Reverse().First();
        }

        public Nothing NextCallOperation(INextCallOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing ElseOperation(IElseOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing MultiplyOperation(IMultiplyOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing IfTrueOperation(IIfOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing LastCallOperation(ILastCallOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }
        public Nothing LessThanOperation(ILessThanOperation co)
        {
            Push(co).Walk(co.Operands);
            return new Nothing();
        }

        // deadends:
        public Nothing ConstantBool(IConstantBool constantBool) => new Nothing();
        public Nothing ConstantNumber(IConstantNumber codeElement) => new Nothing();
        public Nothing ConstantString(IConstantString co) => new Nothing();
        public Nothing EmptyInstance(IEmptyInstance co) => new Nothing();
        public Nothing MemberReferance(IMemberReference codeElement) {
            codeElement.MemberDefinition.Convert(Push(codeElement));
            return new Nothing(); 
        }
        public Nothing MemberDefinition(IMemberDefinition codeElement)
        {
            if (codeElement.Type is IInterfaceType interfaceType)
            {
                // I guess member definition is not a code elemeht 
                interfaceType.Convert(this);
            }
            if (codeElement.Type is ITypeOr typeOr)
            {
                // I guess member definition is not a code elemeht 
                TypeOr(typeOr);
            }
            return new Nothing();
        }

        // TODO these two method (TypeDefinition and TypeOr) don't make me feel good about the design
        // one is on the interface the other is not
        // I am using this weird switch (in MemberDefinition) to get to them
        // everywhere else I use polymorphism to get there 

        public Nothing TypeOr(ITypeOr typeOr)
        {
            foreach (var member in typeOr.Members)
            {
                lookup.TryAddTacField(OrType.Make<IObjectDefiniton, IInterfaceType, ITypeOr>(typeOr), member);
            }

            return new Nothing();
        }

        public Nothing TypeDefinition(IInterfaceType codeElement)
        {
            foreach (var member in codeElement.Members)
            {
                lookup.TryAddTacField(OrType.Make<IObjectDefiniton, IInterfaceType, ITypeOr>(codeElement), member);
            }

            return new Nothing();
        }

        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            if (entryPointDefinition.Scope.Members.Values.Any(x => x.Static))
            {
                throw new Exception("cant be static at this time");
            }


            var next = Push(entryPointDefinition);

            entryPointDefinition.ParameterDefinition.Convert(next);
            if (entryPointDefinition.OutputType.SafeIs(out IInterfaceType type))
            {
                type.Convert(next);
            }

            foreach (var entry in entryPointDefinition.Scope.Members.Values.Select(x => x.Value).Except(new[] { entryPointDefinition.ParameterDefinition }))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(entryPointDefinition), entry);
            }
            lookup.AddArgument(OrType.Make<IImplementationDefinition, IInternalMethodDefinition, IEntryPointDefinition>(entryPointDefinition), entryPointDefinition.ParameterDefinition);


            if (extensionLookup.entryPointLookup.TryGetValue(entryPointDefinition, out var found))
            {
                foreach (var member in found.closureMember.Keys)
                {
                    lookup.AddField(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(entryPointDefinition), member);
                }
            }

            next.Walk(entryPointDefinition.Body);

            return new Nothing();
        }

        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            var owner = GetOwner();

            foreach (var entry in codeElement.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddLocal(owner, entry);
            }


            var next = Push(codeElement);
            next.Walk(codeElement.Body);

            return new Nothing();
        }

        public Nothing MethodDefinition(IInternalMethodDefinition co)
        {
            var next = Push(co);

            co.ParameterDefinition.Convert(next);
            if (co.OutputType.SafeIs(out IInterfaceType type)) {
                type.Convert(next);
            }

            foreach (var entry in co.Scope.Members.Values.Select(x => x.Value).Except(new[] { co.ParameterDefinition }))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(co), entry);
            }
            lookup.AddArgument(OrType.Make<IImplementationDefinition, IInternalMethodDefinition,IEntryPointDefinition>(co), co.ParameterDefinition);

            if (extensionLookup.methodLookup.TryGetValue(co, out var found))
            {
                foreach (var member in found.closureMember.Keys)
                {
                    lookup.AddField(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(co), member);
                }
            }

            next.Walk(co.Body);

            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            var next = Push(codeElement);

            codeElement.ParameterDefinition.Convert(next);
            codeElement.ContextDefinition.Convert(next);
            if (codeElement.OutputType.SafeIs(out IInterfaceType type))
            {
                type.Convert(next);
            }

            foreach (var entry in codeElement.Scope.Members.Values.Select(x => x.Value).Except(new[] { codeElement.ParameterDefinition }))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(codeElement), entry);
            }

            lookup.AddArgument(OrType.Make < IImplementationDefinition, IInternalMethodDefinition,IEntryPointDefinition> (codeElement),codeElement.ParameterDefinition);
            lookup.AddField(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>( codeElement), codeElement.ContextDefinition);

            if (extensionLookup.implementationLookup.TryGetValue(codeElement, out var found)) {

                foreach (var member in found.closureMember.Keys)
                {
                    lookup.AddField(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(codeElement), member);
                } 
            }

            next.Walk(codeElement.MethodBody);

            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            if (codeElement.Scope.Members.Values.Any(x => x.Static))
            {
                // atleast not right now
                throw new Exception("a member can't be static");
            }

            foreach (var member in codeElement.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddTacField(OrType.Make<IObjectDefiniton, IInterfaceType, ITypeOr>(codeElement), member);
            }

            var next = Push(codeElement);
            next.Walk(codeElement.Assignments);

            return new Nothing();
        }

        public Nothing RootScope(IRootScope co)
        {
            foreach (var entry in co.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>(co), entry);
            }
            var next = Push(co);
            next.Walk(co.Assignments);
            co.EntryPoint.Convert(next);

            return new Nothing();
        }

        public Nothing GenericMethodDefinition(IGenericMethodDefinition co)
        {
            throw new NotImplementedException();
        }
    }
}
