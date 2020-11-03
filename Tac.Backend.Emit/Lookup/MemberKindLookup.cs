using Prototypist.TaskChain;
using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tac.Backend.Emit.Visitors;
using Tac.Model;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Lookup
{
 
    
    class MemberKindLookup
    {
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>> locals = new ConcurrentIndexed<IMemberDefinition, IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope>>();
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IInternalMethodDefinition>> arguments = new ConcurrentIndexed<IMemberDefinition, IOrType<IImplementationDefinition, IInternalMethodDefinition>>();
        private readonly ConcurrentIndexed<IMemberDefinition, List<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>>> fields = new ConcurrentIndexed<IMemberDefinition, List<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>>>();
        private readonly ConcurrentIndexed<IMemberDefinition, IOrType<IObjectDefiniton, IInterfaceType, ITypeOr>> tacFields = new ConcurrentIndexed<IMemberDefinition, IOrType<IObjectDefiniton, IInterfaceType, ITypeOr>>();
        private readonly ConcurrentIndexed<IMemberDefinition, FieldInfo> staticFields = new ConcurrentIndexed<IMemberDefinition, FieldInfo>();

        internal void AddLocal(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> owner, IMemberDefinition value)
        {
            locals.AddOrThrow(value, owner);
        }

        internal void AddArgument(IOrType<IImplementationDefinition, IInternalMethodDefinition> codeElement, IMemberDefinition parameterDefinition)
        {
            arguments.AddOrThrow(parameterDefinition, codeElement);
        }

        internal void AddTacField(IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> codeElement, IMemberDefinition contextDefinition)
        {
            tacFields.AddOrThrow(contextDefinition, codeElement);
        }

        internal void AddField(IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition> codeElement, IMemberDefinition contextDefinition)
        {
            fields.TryAdd(contextDefinition, new List<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>>());

            var list = fields.GetOrThrow(contextDefinition);

            if (!list.Contains(codeElement)) {
                list.Add(codeElement);
            }
        }

        internal void TryAddTacField(IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> codeElement, IMemberDefinition contextDefinition)
        {
            tacFields.TryAdd(contextDefinition, codeElement);
        }

        internal void TryAddField(IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> codeElement, IMemberDefinition contextDefinition)
        {
            tacFields.TryAdd(contextDefinition, codeElement);
        }

        internal void AddStaticField(FieldInfo fieldInfo, IMemberDefinition member)
        {
            staticFields.AddOrThrow(member, fieldInfo);
        }

        internal bool IsLocal(IMemberDefinition member, out IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition, IRootScope> orType)
        {
            return locals.TryGetValue(member, out orType);
        }

        internal bool IsArgument(IMemberDefinition member, out IOrType<IImplementationDefinition, IInternalMethodDefinition> orType)
        {
            return arguments.TryGetValue(member, out orType);
        }
        internal bool IsField(IMemberDefinition member, out List<IOrType<IEntryPointDefinition, IImplementationDefinition, IInternalMethodDefinition>> orType)
        {
            return fields.TryGetValue(member, out orType);
        }
        internal bool IsTacField(IMemberDefinition member, out IOrType<IObjectDefiniton, IInterfaceType, ITypeOr> orType) {

            return tacFields.TryGetValue(member, out orType);
        }

        internal bool IsStaticField(IMemberDefinition member, out FieldInfo module)
        {
            return staticFields.TryGetValue(member, out module);
        }
    }
}


// so first you walk up the stack in the closures
// if one fits...
// otherwise you look here 
// 

// field order:
//      for methods:
//          fields:
//              closure in abc order
//          args:
//              there is only one
//          locals:
//              in abs order we need to explicitly exculde the 
//      for imps
//          fields:
//              closure in abc order
//              context
//          args:
//              there is only one
//          locals:
//              in abs order

// actually field order does not appear to matter, I guess you look those up by name...