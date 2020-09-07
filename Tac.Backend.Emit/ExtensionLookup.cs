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
        private readonly Dictionary<IInternalMethodDefinition, MethodExtension> methodLookup = new Dictionary<IInternalMethodDefinition, MethodExtension>();
        public MethodExtension LookUpOrThrow(IInternalMethodDefinition methodDefinition) {
            return methodLookup[methodDefinition];
        }

        public MethodExtension LookUpOrAdd(IInternalMethodDefinition methodDefinition)
        {
            return methodLookup.GetOrAdd(methodDefinition, new MethodExtension());
        }

        private readonly Dictionary<IImplementationDefinition, ImplementationExtension> implementationLookup = new Dictionary<IImplementationDefinition, ImplementationExtension>();
        public ImplementationExtension LookUpOrThrow(IImplementationDefinition implementationDefinition)
        {
            return implementationLookup[implementationDefinition];
        }

        public ImplementationExtension LookUpOrAdd(IImplementationDefinition implementationDefinition)
        {
            return implementationLookup.GetOrAdd(implementationDefinition,new ImplementationExtension());
        }
    }
}
