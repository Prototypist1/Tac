﻿using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed class ParameterDefinition: IReferanced
    {
        public ParameterDefinition(bool readOnly, TypeReferance type, AbstractName key)
        {
            ReadOnly = readOnly;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public bool ReadOnly { get; }
        public TypeReferance Type { get; }
        public AbstractName Key { get; }

        public override bool Equals(object obj)
        {
            return obj is ParameterDefinition definition && definition != null &&
                   ReadOnly == definition.ReadOnly &&
                   EqualityComparer<TypeReferance>.Default.Equals(Type, definition.Type) &&
                   EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1232917096;
            hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TypeReferance>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            return hashCode;
        }
    }
}