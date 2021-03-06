﻿using Prototypist.Toolbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Tac.Backend.Emit.Visitors;
using Tac.Model;
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

        internal void CreateTypes()
        {
            foreach (var realizedMethod in backing.Values)
            {
                realizedMethod.type.CreateType();
            } 
        }
    }

    internal class RealizedMethod {

        public readonly IReadOnlyDictionary<IMemberDefinition, IOrType< FieldInfo,(FieldInfo funcField, FieldInfo path), EnclosedObjectMember>> fieldOrFieldPair;
        public readonly TypeBuilder type;
        public readonly ConstructorBuilder defaultConstructor;

        public RealizedMethod(IReadOnlyDictionary<IMemberDefinition, IOrType<FieldInfo, (FieldInfo funcField, FieldInfo path), EnclosedObjectMember>> fieldOrFieldPair, TypeBuilder type, ConstructorBuilder defaultConstructor)
        {
            this.fieldOrFieldPair = fieldOrFieldPair ?? throw new ArgumentNullException(nameof(fieldOrFieldPair));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.defaultConstructor = defaultConstructor ?? throw new ArgumentNullException(nameof(defaultConstructor));
        }
    }
}
