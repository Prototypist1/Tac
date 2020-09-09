using Prototypist.Toolbox.Dictionary;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Backend.Emit.Extensions;
using Tac.Model.Elements;

namespace Tac.Backend.Emit
{
    class ExtensionLookup
    {
        public readonly Dictionary<IInternalMethodDefinition, ClosureExtension> methodLookup = new Dictionary<IInternalMethodDefinition, ClosureExtension>();

        //public ClosureExtension LookUpOrAdd(IInternalMethodDefinition methodDefinition)
        //{
        //    return methodLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}

        public readonly Dictionary<IImplementationDefinition, ClosureExtension> implementationLookup = new Dictionary<IImplementationDefinition, ClosureExtension>();


        //public ClosureExtension LookUpOrAdd(IImplementationDefinition implementationDefinition)
        //{
        //    return implementationLookup.GetOrAdd(implementationDefinition,new ClosureExtension());
        //}


        // I really doubt blocks need closures
        public readonly Dictionary<IBlockDefinition, ClosureExtension> blockLookup = new Dictionary<IBlockDefinition, ClosureExtension>();

        //public ClosureExtension LookUpOrAdd(IBlockDefinition methodDefinition)
        //{
        //    return blockLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}

        public readonly Dictionary<IEntryPointDefinition, ClosureExtension> entryPointLookup = new Dictionary<IEntryPointDefinition, ClosureExtension>();

        //public ClosureExtension LookUpOrAdd(IEntryPointDefinition methodDefinition)
        //{
        //    return entryPointLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        //}
    }
}
