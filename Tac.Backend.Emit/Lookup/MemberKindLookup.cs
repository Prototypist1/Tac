using Prototypist.TaskChain;
using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Lookup
{
    class MemberKindLookup
    {
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>> locals = new ConcurrentIndexed<IMemberDefinition, IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>>();
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IInternalMethodDefinition>> arguments = new ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IInternalMethodDefinition>>();
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IObjectDefiniton>> field = new ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IObjectDefiniton>>();
        private readonly ConcurrentIndexed<IMemberDefinition, IModuleDefinition> staticField = new ConcurrentIndexed<IMemberDefinition, IModuleDefinition>();

        internal void AddLocal(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition> owner, IMemberDefinition value)
        {
            locals.AddOrThrow(value, owner);
        }

        internal void AddArgument(IOrType<IImplementationDefinition, IInternalMethodDefinition> codeElement, IMemberDefinition parameterDefinition)
        {
            arguments.AddOrThrow( parameterDefinition, codeElement);
        }

        internal void AddField(IOrType<IImplementationDefinition, IObjectDefiniton> codeElement, IMemberDefinition contextDefinition)
        {
            field.AddOrThrow(contextDefinition, codeElement);
        }

        internal void AddStaticField(IModuleDefinition codeElement, IMemberDefinition member)
        {
            staticField.AddOrThrow(member, codeElement);
        }
    }
}
