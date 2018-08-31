using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface ITypeSource {
        bool TryGetTypeDefinition(ScopeScope scope, out ITypeDefinition typeDefinition);
    }

    //public sealed class ParameterDefinition: IReferanced
    //{
    //    public ParameterDefinition(bool readOnly, ITypeSource type, AbstractName key)
    //    {
    //        ReadOnly = readOnly;
    //        Type = type ?? throw new ArgumentNullException(nameof(type));
    //        Key = key ?? throw new ArgumentNullException(nameof(key));
    //    }

    //    public bool ReadOnly { get; }
    //    public ITypeSource Type { get; }
    //    public AbstractName Key { get; }

    //    public override bool Equals(object obj)
    //    {
    //        return obj is ParameterDefinition definition && definition != null &&
    //               ReadOnly == definition.ReadOnly &&
    //               EqualityComparer<ITypeSource>.Default.Equals(Type, definition.Type) &&
    //               EqualityComparer<AbstractName>.Default.Equals(Key, definition.Key);
    //    }

    //    public override int GetHashCode()
    //    {
    //        var hashCode = 1232917096;
    //        hashCode = hashCode * -1521134295 + ReadOnly.GetHashCode();
    //        hashCode = hashCode * -1521134295 + EqualityComparer<ITypeSource>.Default.GetHashCode(Type);
    //        hashCode = hashCode * -1521134295 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
    //        return hashCode;
    //    }

    //    public ITypeDefinition ReturnType(ScopeStack scope) {
    //        if (Type.TryGetTypeDefinition(scope, out var res))
    //        {
    //            return res;
    //        }

    //        throw new Exception("return type not found");
            
    //    }
    //}
}