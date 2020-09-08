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
        private readonly Dictionary<IInternalMethodDefinition, ClosureExtension> methodLookup = new Dictionary<IInternalMethodDefinition, ClosureExtension>();
        //public MethodExtension LookUpOrThrow(IInternalMethodDefinition methodDefinition) {
        //    return methodLookup[methodDefinition];
        //}

        public ClosureExtension LookUpOrAdd(IInternalMethodDefinition methodDefinition)
        {
            return methodLookup.GetOrAdd(methodDefinition, new ClosureExtension());
        }

        private readonly Dictionary<IImplementationDefinition, ClosureExtension> implementationLookup = new Dictionary<IImplementationDefinition, ClosureExtension>();
        //public MethodExtension LookUpOrThrow(IImplementationDefinition implementationDefinition)
        //{
        //    return implementationLookup[implementationDefinition];
        //}

        public ClosureExtension LookUpOrAdd(IImplementationDefinition implementationDefinition)
        {
            return implementationLookup.GetOrAdd(implementationDefinition,new ClosureExtension());
        }
    }
}
