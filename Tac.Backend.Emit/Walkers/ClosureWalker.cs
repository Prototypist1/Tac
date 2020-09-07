using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Operations;

namespace Tac.Backend.Emit.Walkers
{
    class ClosureWalker
    {

        public void Walk(ICodeElement root, ExtensionLookup extensionLookup) {
            WalkSignle(root, new List<IFinalizedScope>(), extensionLookup);
        }

        private List<IMemberDefinition> Walk(IEnumerable<ICodeElement> elements, IReadOnlyList<IFinalizedScope> scopeStack, ExtensionLookup extensionLookup)
        {

            var closure = new List<IMemberDefinition>();

            foreach (var line in elements)
            {
                closure = closure.Union(WalkSignle(line, scopeStack, extensionLookup)).ToList();
            }

            return closure;
        }

        private List<IMemberDefinition> WalkSignle(ICodeElement line, IReadOnlyList<IFinalizedScope> scopeStack, ExtensionLookup extensionLookup) {

            var closure = new List<IMemberDefinition>();

            if (line.SafeIs(out IPathOperation path))
            {

                // we are not interested in the second member
                if (path.Operands.Count != 2)
                {
                    throw new Exception("i am assuming ther e are 2 parms and the second is a member reference");
                }

                if (!path.Right.SafeIs(out IMemberReferance referance))
                {
                    throw new Exception("i am assuming ther e are 2 parms and the second is a member reference");
                }

                closure = closure.Union(Walk(path.Operands.Take(1), scopeStack, extensionLookup)).ToList();


            }
            else if (line.SafeIs(out IInternalMethodDefinition method))
            {
                var nextScopeStack = scopeStack.ToList();

                nextScopeStack.Add(method.Scope);

                if (method.StaticInitailizers.Any())
                {
                    throw new Exception("I don't undterstand why a method would have static initailizers, if this exception is hit then you are in a postition to find out");
                }

                var methodClosure = Walk(method.Body, nextScopeStack, extensionLookup);

                var methodExtension = extensionLookup.LookUpOrAdd(method);

                if (!methodExtension.closureMember.Any())
                {
                    throw new Exception("this should not already be populated");
                }
                methodExtension.closureMember.AddRange(methodClosure);

                closure = closure.Union(methodClosure).ToList();

            }
            else if (line.SafeIs(out IObjectDefiniton @object))
            {
                var nextScopeStack = scopeStack.ToList();

                nextScopeStack.Add(@object.Scope);


                closure = closure.Union(Walk(@object.Assignments, nextScopeStack, extensionLookup)).ToList();

            }
            else if (line.SafeIs(out IModuleDefinition module))
            {
                var nextScopeStack = scopeStack.ToList();

                nextScopeStack.Add(module.Scope);


                closure = closure.Union(Walk(module.StaticInitialization, nextScopeStack, extensionLookup)).ToList();

            }
            else if (line.SafeIs(out IImplementationDefinition implementation))
            {
                var nextScopeStack = scopeStack.ToList();

                nextScopeStack.Add(implementation.IntermediateScope);
                nextScopeStack.Add(implementation.Scope);

                var implementationClosure = Walk(method.Body, nextScopeStack, extensionLookup);

                var implementationExtension = extensionLookup.LookUpOrAdd(implementation);

                if (!implementationExtension.closureMember.Any())
                {
                    throw new Exception("this should not already be populated");
                }
                implementationExtension.closureMember.AddRange(implementationClosure);

                closure = closure.Union(implementationClosure).ToList();
            }
            else if (line.SafeIs(out IMemberReferance memberReferance))
            {
                foreach (var scope in scopeStack.Reverse())
                {
                    if (scope.Members.Any(x => x.Value.Value == memberReferance.MemberDefinition))
                    {
                        if (!closure.Contains(memberReferance.MemberDefinition))
                        {
                            closure.Add(memberReferance.MemberDefinition);
                        }
                        goto done;
                    }
                }
            done:;
            }
            else if (line.SafeIs(out IOperation op))
            {
                closure = closure.Union(Walk(op.Operands, scopeStack, extensionLookup)).ToList();
            }

            return closure;
        }
    }
}
