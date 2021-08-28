using Prototypist.Toolbox;
using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
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

        public IOrType<IKey, IError>[] Types { get; }

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

    // double generic names keys exist for generic methods
    //
    // generic-method [T] [T,T] y := generic-method [T] [T,T] input { input return; }
    // x realize [number] < 5 
    // 
    // "generic-method [T] [T,T]" is our double generic key
    //
    // conceptually this is something that might need a bit more proving out
    // the second set of generics depend of the first
    // 
    public class DoubleGenericNameKey : IKey
    {
        public NameKey Name { get; }
        public DoubleGenericNameKey(NameKey name, IOrType<IKey, IError>[] types, IOrType<IKey, IError>[] dependentTypes)
        {
            this.Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Types = types ?? throw new System.ArgumentNullException(nameof(types));
            DependentTypes = dependentTypes ?? throw new ArgumentNullException(nameof(dependentTypes));
        }

        public IOrType<IKey, IError>[] Types { get; }
        public IOrType<IKey, IError>[] DependentTypes { get; }

        public override string ToString()
        {
            return $"{nameof(DoubleGenericNameKey)}-{Name.ToString()}-{Types.Aggregate("", (x, y) => x + "" + y.ToString())}-{DependentTypes.Aggregate("", (x, y) => x + "" + y.ToString())}";
        }

        public override bool Equals(object? obj)
        {
            return obj is DoubleGenericNameKey key &&
                   EqualityComparer<NameKey>.Default.Equals(Name, key.Name) &&
                   EqualityComparer<IOrType<IKey, IError>[]>.Default.Equals(Types, key.Types) &&
                   EqualityComparer<IOrType<IKey, IError>[]>.Default.Equals(DependentTypes, key.DependentTypes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Types, DependentTypes);
        }
    }
}
