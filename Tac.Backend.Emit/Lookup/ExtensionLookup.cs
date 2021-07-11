using Prototypist.TaskChain;
using Prototypist.Toolbox.Dictionary;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Emit.Extensions;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Backend.Emit
{
    class WhoDefinedMemberByMethodlike
    {
        public readonly ConcurrentIndexed<IInternalMethodDefinition, WhoDefinedMember> methodLookup = new ConcurrentIndexed<IInternalMethodDefinition, WhoDefinedMember>();

        //public ClosureExtension LookUpOrAdd(IInternalMethodDefinition methodDefinition)
        //{
        //    return methodLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}

        public readonly ConcurrentIndexed<IImplementationDefinition, WhoDefinedMember> implementationLookup = new ConcurrentIndexed<IImplementationDefinition, WhoDefinedMember>();


        //public ClosureExtension LookUpOrAdd(IImplementationDefinition implementationDefinition)
        //{
        //    return implementationLookup.GetOrAdd(implementationDefinition,new ClosureExtension());
        //}


        // I really doubt blocks need closures
        //public readonly ConcurrentIndexed<IBlockDefinition, ClosureLookup> blockLookup = new ConcurrentIndexed<IBlockDefinition, ClosureLookup>();

        //public ClosureExtension LookUpOrAdd(IBlockDefinition methodDefinition)
        //{
        //    return blockLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}

            
        public readonly ConcurrentIndexed<IEntryPointDefinition, WhoDefinedMember> entryPointLookup = new ConcurrentIndexed<IEntryPointDefinition, WhoDefinedMember>();

        //public ClosureExtension LookUpOrAdd(IEntryPointDefinition methodDefinition)
        //{
        //    return entryPointLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}


        internal bool InAnyClosure(IMemberDefinition memberDefinition) {
            foreach (var item in methodLookup.Values)
            {
                foreach (var member in item.closureMember)
                {
                    if (memberDefinition == member.Key) {
                        return true;
                    }
                }
            }

            foreach (var item in implementationLookup.Values)
            {
                foreach (var member in item.closureMember)
                {
                    if (memberDefinition == member.Key)
                    {
                        return true;
                    }
                }
            }

            foreach (var item in entryPointLookup.Values)
            {
                foreach (var member in item.closureMember)
                {
                    if (memberDefinition == member.Key)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal bool TryGetClosure(ICodeElement codeElement, out WhoDefinedMember closureLookup) {
            if (codeElement.SafeIs(out IInternalMethodDefinition methodDefinition) && methodLookup.TryGetValue(methodDefinition, out closureLookup)) {
                return true;
            }
            if (codeElement.SafeIs(out IImplementationDefinition implementationDefinition) && implementationLookup.TryGetValue(implementationDefinition, out closureLookup))
            {
                return true;
            }
            //if (codeElement.SafeIs(out IBlockDefinition blockDefinition) && blockLookup.TryGetValue(blockDefinition, out closureLookup))
            //{
            //    return true;
            //}
            if (codeElement.SafeIs(out IEntryPointDefinition entryPointDefinition) && entryPointLookup.TryGetValue(entryPointDefinition, out closureLookup))
            {
                return true;
            }
            closureLookup = default;
            return false;
        }
    }
}
