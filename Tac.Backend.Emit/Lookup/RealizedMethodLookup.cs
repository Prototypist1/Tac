using Prototypist.Toolbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Lookup
{
    internal class RealizedMethodLookup
    {
        private readonly ConcurrentDictionary<IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>, RealizedMethod> backing = new ConcurrentDictionary<IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition>, RealizedMethod>();

        internal void Add(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType, RealizedMethod realizedMethod)
        {
            if (!backing.TryAdd(orType, realizedMethod))
            {
                throw new Exception("should have added, I think");
            }
        }

        internal bool TryGetValue(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType, out RealizedMethod realizedMethod)
        {
            return backing.TryGetValue(orType, out realizedMethod);
        }

        internal RealizedMethod GetValueOrThrow(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType)
        {
            if (backing.TryGetValue(orType, out var res)) {
                return res;
            }
            throw new Exception("key not found");
        }
    }

    internal class RealizedMethod {
        public readonly IReadOnlyDictionary<IMemberDefinition,FieldInfo> fields;
        public readonly TypeBuilder type;

        public RealizedMethod(IReadOnlyDictionary<IMemberDefinition, FieldInfo> fields, TypeBuilder type)
        {
            this.fields = fields ?? throw new ArgumentNullException(nameof(fields));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
