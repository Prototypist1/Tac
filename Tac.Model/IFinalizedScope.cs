using System;
using System.Collections.Generic;
using Tac.Model.WithErrors.Elements;
using System.Linq;

namespace Tac.Model.WithErrors
{
    public class TypeEntry {
        public TypeEntry(IKey key, IVerifiableType type)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IKey Key { get; }
        public IVerifiableType Type { get; }

        public override bool Equals(object obj)
        {
            return obj is TypeEntry entry &&
                   EqualityComparer<IKey>.Default.Equals(Key, entry.Key) &&
                   EqualityComparer<IVerifiableType>.Default.Equals(Type, entry.Type);
        }

        public override int GetHashCode()
        {
            var hashCode = 207421273;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IKey>.Default.GetHashCode(Key);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IVerifiableType>.Default.GetHashCode(Type);
            return hashCode;
        }
    }


    // TODO very confusing name

    // honestly this is a pretty bad interface
    // I was clearly adding what I needed as I went 
    public interface IFinalizedScope
    {
        IReadOnlyDictionary<IKey, IsStatic> Members { get; }

    }

    public struct IsStatic
    {
        public IsStatic(IMemberDefinition value, bool @static)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Static = @static;
        }

        public IMemberDefinition Value { get; }
        public bool Static { get; }
    }

    public class GenericKeyDefinition : IKey
    {
        public GenericKeyDefinition(NameKey name, IReadOnlyList<IKey> parameters)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public NameKey Name { get; }
        public IReadOnlyList<IKey> Parameters { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericKeyDefinition definition &&
                   EqualityComparer<NameKey>.Default.Equals(Name, definition.Name) &&
                   Parameters.SequenceEqual(definition.Parameters);
        }

        public override int GetHashCode()
        {
            var hashCode = 497090031;
            hashCode = (hashCode * -1521134295) + EqualityComparer<NameKey>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + Parameters.Sum(x=>x.GetHashCode());
            return hashCode;
        }
    }

}