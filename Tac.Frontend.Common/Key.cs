﻿using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Linq;

namespace Tac.Model
{

    public class ImplicitKey : IKey, IEquatable<ImplicitKey>
    {
        private readonly Guid guid;

        public ImplicitKey(Guid guid)
        {
            this.guid = guid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj.SafeCastTo<object,ImplicitKey>());
        }

        public bool Equals(ImplicitKey other)
        {
            return other != null &&
                   guid.Equals(other.guid);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(guid);
        }
    }

    public class GenericNameKey : IKey
    {
        public  NameKey Name { get; }
        public GenericNameKey(NameKey name, IOrType<IKey,IError>[] types)
        {
            this.Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
        }

        public IKey[] Types { get; }

        public override bool Equals(object obj)
        {
            return obj is GenericNameKey key &&
                   base.Equals(obj) &&
                   Types.SequenceEqual(key.Types);
        }

        public override int GetHashCode()
        {
            var hashCode = -850890288;
            hashCode = (hashCode * -1521134295) + base.GetHashCode();
            hashCode = (hashCode * -1521134295) + Types.Sum(x=>x.GetHashCode());
            return hashCode;
        }

        public override string ToString()
        {
            return $"{nameof(GenericNameKey)}-{Name.ToString()}-{Types.Aggregate("",(x,y)=> x +""+ y.ToString())}";
        }
    }
}
