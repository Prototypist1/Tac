using System;
using System.Linq;

namespace Tac.Semantic_Model
{
    public sealed partial class RootScope
    {
        private class GenericFundimentalType : FundimentalType
        {
            public GenericFundimentalType(string key, params ITypeDefinition<IScope>[] typeDefinition) : base(key) => TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));

            public ITypeDefinition<IScope>[] TypeDefinition { get; }

            public override bool Equals(object obj)
            {
                var type = obj as GenericFundimentalType;
                return type != null &&
                       base.Equals(obj) &&
                       TypeDefinition.SequenceEqual(type.TypeDefinition);
            }

            public override int GetHashCode()
            {
                var hashCode = 1723393694;
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + TypeDefinition.Sum(x=>x.GetHashCode());
                return hashCode;
            }
        }
        
        public static ITypeDefinition<IScope> MethodType(ITypeDefinition<IScope> input, ITypeDefinition<IScope> output)
        {
            return new GenericFundimentalType("Method", input, output);
        }
        public static ITypeDefinition<IScope> ImplementationType(ITypeDefinition<IScope> context, ITypeDefinition<IScope> input, ITypeDefinition<IScope> output)
        {
            return new GenericFundimentalType("Implementation", context, input, output);
        }
    }

}