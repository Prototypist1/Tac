using System;
using System.Collections.Generic;

namespace Tac.Model
{
    public class NameKey : IKey
    {
        public NameKey(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            return obj is NameKey key &&
                   Name == key.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return $"{nameof(NameKey)}-{Name}";
        }

        //public IKey Replace((IKey, IKey)[] replacements)
        //{
        //    foreach (var (from, to) in replacements)
        //    {
        //        if (from.Equals(this))
        //        {
        //            return to;
        //        }
        //    }

        //    return this;
        //}
    }

}
