using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Backend.Emit.Extensions;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{
    class ClosureWalker : IOpenBoxesContext<IReadOnlyList<IMemberDefinition>>
    {
        private readonly ExtensionLookup extensionLookup;

        public ClosureWalker(ExtensionLookup extensionLookup)
        {
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
        }

        public IReadOnlyList<IMemberDefinition> AddOperation(IAddOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> AssignOperation(IAssignOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> BlockDefinition(IBlockDefinition codeElement)
        {
            return extensionLookup.blockLookup.GetOrAdd(codeElement,()=> {
                var implementationClosure = Walk(codeElement.Body, extensionLookup);

                return new ClosureExtension(implementationClosure
                .Except(codeElement.Scope.Members.Select(x => x.Value.Value)).ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> ConstantBool(IConstantBool constantBool) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> ConstantNumber(IConstantNumber codeElement) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> ConstantString(IConstantString co) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> EmptyInstance(IEmptyInstance co) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> TypeDefinition(IInterfaceType codeElement) => new List<IMemberDefinition>();


        public IReadOnlyList<IMemberDefinition> ElseOperation(IElseOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }


        public IReadOnlyList<IMemberDefinition> EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            return extensionLookup.entryPointLookup.GetOrAdd(entryPointDefinition, () => {
                var implementationClosure = Walk(entryPointDefinition.Body, extensionLookup);

                return new ClosureExtension(implementationClosure
                .Except(entryPointDefinition.Scope.Members.Select(x => x.Value.Value)).ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> IfTrueOperation(IIfOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> ImplementationDefinition(IImplementationDefinition implementation)
        {
            return extensionLookup.implementationLookup.GetOrAdd(implementation, () => {
                var implementationClosure = Walk(implementation.MethodBody, extensionLookup);

                return new ClosureExtension(implementationClosure
                    .Except(implementation.IntermediateScope.Members.Select(x => x.Value.Value))
                    .Except(implementation.Scope.Members.Select(x => x.Value.Value)).ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> LastCallOperation(ILastCallOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> LessThanOperation(ILessThanOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> MemberDefinition(IMemberDefinition codeElement)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IMemberDefinition> MemberReferance(IMemberReferance memberReferance)
        {
            return new List<IMemberDefinition> { memberReferance.MemberDefinition };
        }

        public IReadOnlyList<IMemberDefinition> MethodDefinition(IInternalMethodDefinition method)
        {
            return extensionLookup.methodLookup.GetOrAdd(method, () => {
                var implementationClosure = Walk(method.Body, extensionLookup);

                return new ClosureExtension(implementationClosure
                .Except(method.Scope.Members.Select(x => x.Value.Value)).ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> ModuleDefinition(IModuleDefinition module)
        {
            return Walk(module.StaticInitialization, extensionLookup).ToList();
        }

        public IReadOnlyList<IMemberDefinition> MultiplyOperation(IMultiplyOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> NextCallOperation(INextCallOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> ObjectDefinition(IObjectDefiniton @object)
        {
            return Walk(@object.Assignments, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> PathOperation(IPathOperation path)
        {

            return path.Operands.First().Convert(this);
        }

        public IReadOnlyList<IMemberDefinition> ReturnOperation(IReturnOperation co)
        {
            return co.Result.Convert(this);
        }

        public IReadOnlyList<IMemberDefinition> SubtractOperation(ISubtractOperation co)
        {
            return Walk(co.Operands, extensionLookup);
        }

        public IReadOnlyList<IMemberDefinition> TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {
            return Walk(tryAssignOperation.Operands, extensionLookup);
        }


        private IReadOnlyList<IMemberDefinition> Walk(IEnumerable<ICodeElement> elements, ExtensionLookup extensionLookup)
        {

            var closure = new List<IMemberDefinition>();

            foreach (var line in elements)
            {
                closure = closure.Union(line.Convert(this)).ToList();
            }

            return closure;
        }
    }
}
