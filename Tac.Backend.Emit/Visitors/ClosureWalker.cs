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
    // {CC9EC96A-4DD8-4E0D-A016-3E6BB1320DE9}
    // does this handle static well?


    // does this handle argument well?
    // they should not be part of the closure
    // do argument endup as part of the 

    class ClosureVisitor : IOpenBoxesContext<IReadOnlyList<IMemberDefinition>>
    {
        private readonly ExtensionLookup extensionLookup;

        public ClosureVisitor(ExtensionLookup extensionLookup)
        {
            this.extensionLookup = extensionLookup ?? throw new ArgumentNullException(nameof(extensionLookup));
        }

        public IReadOnlyList<IMemberDefinition> AddOperation(IAddOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> AssignOperation(IAssignOperation co)
        {
            return Walk(co.Operands);
        }


        public IReadOnlyList<IMemberDefinition> TryAssignOperation(ITryAssignOperation tryAssignOperation)
        {

            var implementationClosure = Walk(tryAssignOperation.Operands);

            return implementationClosure
            .Except(tryAssignOperation.Scope.Members.Select(x => x.Value.Value)).ToArray();

        }

        public IReadOnlyList<IMemberDefinition> BlockDefinition(IBlockDefinition codeElement)
        {

            var implementationClosure = Walk(codeElement.Body);

            return implementationClosure
            .Except(codeElement.Scope.Members.Select(x => x.Value.Value)).ToArray();

            // TODO BLOCKS DONT NEED CLOSURES
            //return extensionLookup.blockLookup.GetOrAdd(codeElement,()=> {
            //    var implementationClosure = Walk(codeElement.Body, extensionLookup);

            //    return new ClosureLookup(implementationClosure
            //    .Except(codeElement.Scope.Members.Select(x => x.Value.Value)).ToArray());

            //}).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> ConstantBool(IConstantBool constantBool) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> ConstantNumber(IConstantNumber codeElement) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> ConstantString(IConstantString co) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> EmptyInstance(IEmptyInstance co) => new List<IMemberDefinition>();
        public IReadOnlyList<IMemberDefinition> TypeDefinition(IInterfaceType codeElement) => new List<IMemberDefinition>();


        public IReadOnlyList<IMemberDefinition> ElseOperation(IElseOperation co)
        {
            return Walk(co.Operands);
        }


        public IReadOnlyList<IMemberDefinition> EntryPoint(IEntryPointDefinition entryPointDefinition)
        {
            // an entry point totally has a closure:
            // x := 2;
            // entrypoint {
            //      x + 1 > some-method;
            // }
            return extensionLookup.entryPointLookup.GetOrAdd(entryPointDefinition, () => {
                var implementationClosure = Walk(entryPointDefinition.Body);

                return new ClosureLookup(implementationClosure
                    .Except(entryPointDefinition.Scope.Members.Select(x => x.Value.Value))
                    .ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> IfTrueOperation(IIfOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> ImplementationDefinition(IImplementationDefinition implementation)
        {
            return extensionLookup.implementationLookup.GetOrAdd(implementation, () => {
                var implementationClosure = Walk(implementation.MethodBody);

                return new ClosureLookup(implementationClosure
                    .Except(implementation.IntermediateScope.Members.Select(x => x.Value.Value))
                    .Except(implementation.Scope.Members.Select(x => x.Value.Value)).ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> LastCallOperation(ILastCallOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> LessThanOperation(ILessThanOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> MemberDefinition(IMemberDefinition codeElement)
        {
            return new List<IMemberDefinition>();
        }

        public IReadOnlyList<IMemberDefinition> MemberReferance(IMemberReference memberReferance)
        {
            // this is only sometimes a closure
            // 
            return new List<IMemberDefinition> { memberReferance.MemberDefinition };
        }

        public IReadOnlyList<IMemberDefinition> MethodDefinition(IInternalMethodDefinition method)
        {
            return extensionLookup.methodLookup.GetOrAdd(method, () => {
                var implementationClosure = Walk(method.Body);

                return new ClosureLookup(implementationClosure
                .Except(method.Scope.Members.Select(x => x.Value.Value))
                .ToArray());

            }).closureMember;
        }

        public IReadOnlyList<IMemberDefinition> MultiplyOperation(IMultiplyOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> NextCallOperation(INextCallOperation co)
        {
            return Walk(co.Operands);
        }

        public IReadOnlyList<IMemberDefinition> ObjectDefinition(IObjectDefiniton @object)
        {
            var membersReferenced= Walk(@object.Assignments);

            return membersReferenced
            .Except(@object.Scope.Members.Select(x => x.Value.Value)).ToArray();
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
            return Walk(co.Operands);
        }



        private IReadOnlyList<IMemberDefinition> Walk(IEnumerable<ICodeElement> elements)
        {

            var closure = new List<IMemberDefinition>();

            foreach (var line in elements)
            {
                closure = closure.Union(line.Convert(this)).ToList();
            }

            return closure;
        }

        public IReadOnlyList<IMemberDefinition> RootScope(IRootScope co)
        {
            foreach (var item in co.Assignments)
            {
                item.Convert(this);
            }
            co.EntryPoint.Convert(this);

            // this can't have a closure 
            return Array.Empty<IMemberDefinition>();
        }
    }
}
