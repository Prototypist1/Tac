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

        private IReadOnlyList<ICodeElement> stack;
        private readonly MemberKindLookup lookup;

        public MemberKindVisitor(IReadOnlyList<ICodeElement> stack, MemberKindLookup lookup)
        {
            this.stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this.lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
        }

        public MemberKindVisitor Push(ICodeElement another)
        {
            var list = stack.ToList();
            list.Add(another);
            return new MemberKindVisitor(list, lookup);
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
            Push(tryAssignOperation).Walk(tryAssignOperation.Operands);
            return new Nothing();
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
        public Nothing MemberReferance(IMemberReference codeElement) => new Nothing();
        public Nothing MemberDefinition(IMemberDefinition codeElement) => new Nothing();
        public Nothing TypeDefinition(IInterfaceType codeElement) => new Nothing();




        public Nothing EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            if (entryPointDefinition.Scope.Members.Values.Any(x => x.Static))
            {
                throw new Exception("cant be static at this time");
            }


            foreach (var entry in entryPointDefinition.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(entryPointDefinition), entry);
            }

            var next = Push(entryPointDefinition);
            next.Walk(entryPointDefinition.Body);

            return new Nothing();
        }
        public Nothing BlockDefinition(IBlockDefinition codeElement)
        {
            // climb the stack until you find a method or implementation or entry point

            var owner = stack.Reverse().SelectMany(x => {
                if (x.SafeIs(out IInternalMethodDefinition method)) {
                    return new[] { OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(method) };
                }
                if (x.SafeIs(out IImplementationDefinition implementation))
                {
                    return new[] { OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(implementation) };
                }
                if (x.SafeIs(out IEntryPointDefinition entry))
                {
                    return new[] { OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(entry) };
                }
                return new IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>[] { };
            }).First();


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
            foreach (var entry in co.Scope.Members.Values.Select(x => x.Value).Except(new[] { co.ParameterDefinition }))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(co), entry);
            }
            lookup.AddArgument(OrType.Make<IImplementationDefinition, IInternalMethodDefinition>(co), co.ParameterDefinition);

            var next = Push(co);
            next.Walk(co.Body);

            return new Nothing();
        }

        public Nothing ImplementationDefinition(IImplementationDefinition codeElement)
        {
            foreach (var entry in codeElement.Scope.Members.Values.Select(x => x.Value).Except(new[] { codeElement.ParameterDefinition }))
            {
                lookup.AddLocal(OrType.Make<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>(codeElement), entry);
            }

            lookup.AddArgument(OrType.Make < IImplementationDefinition, IInternalMethodDefinition > (codeElement),codeElement.ParameterDefinition);
            lookup.AddField(OrType .Make< IImplementationDefinition, IObjectDefiniton >( codeElement), codeElement.ContextDefinition);

            var next = Push(codeElement);
            next.Walk(codeElement.MethodBody);

            return new Nothing();
        }

        public Nothing ModuleDefinition(IModuleDefinition codeElement)
        {
            throw new NotImplementedException();

            //if (codeElement.Scope.Members.Values.Any(x => !x.Static))
            //{
            //    throw new Exception("a modules members are all static");
            //}

            //foreach (var member in codeElement.Scope.Members.Values.Select(x => x.Value))
            //{
            //    lookup.AddStaticField(codeElement, member);
            //}

            //var next = Push(codeElement);
            //next.Walk(codeElement.StaticInitialization);

            return new Nothing();
        }

        public Nothing ObjectDefinition(IObjectDefiniton codeElement)
        {
            if (codeElement.Scope.Members.Values.Any(x => !x.Static))
            {
                // atleast not right now
                throw new Exception("a modules can't be static");
            }

            foreach (var member in codeElement.Scope.Members.Values.Select(x => x.Value))
            {
                lookup.AddField(OrType.Make<IImplementationDefinition, IObjectDefiniton>(codeElement), member);
            }

            var next = Push(codeElement);
            next.Walk(codeElement.Assignments);

            return new Nothing();
        }

    }
}
