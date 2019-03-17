using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using System.Linq;

namespace Tac.Model
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

    public class GenericTypeEntry
    {
        public GenericTypeEntry(IGenericType type, GenericKeyDefinition key)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IGenericType Type { get; }
        public GenericKeyDefinition Key { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericTypeEntry entry &&
                   EqualityComparer<IGenericType>.Default.Equals(Type, entry.Type) &&
                   EqualityComparer<GenericKeyDefinition>.Default.Equals(Key, entry.Key);
        }

        public override int GetHashCode()
        {
            var hashCode = 1195268993;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IGenericType>.Default.GetHashCode(Type);
            hashCode = (hashCode * -1521134295) + EqualityComparer<GenericKeyDefinition>.Default.GetHashCode(Key);
            return hashCode;
        }
    }


    public interface IFinalizedScope
    {
        IReadOnlyList<IMemberDefinition> Members { get; }
        IReadOnlyList<TypeEntry> Types { get; }
        IReadOnlyList<IKey> MemberKeys { get; }
        IReadOnlyList<GenericTypeEntry> GenericTypes { get; }
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