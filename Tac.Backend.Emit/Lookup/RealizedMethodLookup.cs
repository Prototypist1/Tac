using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Model.Elements;

namespace Tac.Backend.Emit.Lookup
{
    internal class RealizedMethodLookup
    {
        internal void Add(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType, RealizedMethod realizedMethod)
        {
            throw new NotImplementedException();
        }

        internal bool TryGetValue(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType, out RealizedMethod realizedMethod)
        {
            throw new NotImplementedException();
        }

        internal RealizedMethod GetValueOrThrow(IOrType<IInternalMethodDefinition, IImplementationDefinition, IEntryPointDefinition> orType)
        {
            throw new NotImplementedException();
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
