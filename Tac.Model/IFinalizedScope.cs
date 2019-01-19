using System;
using System.Collections.Generic;
using Tac.Model.Elements;
using System.Linq;

namespace Tac.Model
{
    // what even is the point of this interface??
    // I want an alis
    // but is it worth the price?
    

    // TODO some scope has a lot of the same members
    // figure scope interface our
    public interface IFinalizedScope
    {
        IEnumerable<IMemberDefinition> Members { get; }
        IEnumerable<IVerifiableType> Types { get; }
        IEnumerable<IKey> TypeKeys { get; }
        IEnumerable<IKey> MemberKeys { get; }
        IEnumerable<IGenericType> GenericTypes { get; }
        IEnumerable<GenericKeyDefinition> GenericTypeKeys { get; }
        //bool TryGetMember(IKey name, bool staticOnly, out IMemberDefinition box);
        //bool TryGetType(IKey name, out IVerifiableType type);
        //bool TryGetParent(out IFinalizedScope res); 
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