using System;
using System.Linq;

namespace Tac.Semantic_Model
{
    public sealed partial class RootScope
    {
        private class GenericFundimentalType : FundimentalType
        {
            public GenericFundimentalType(string key, params ITypeDefinition[] typeDefinition) : base(key) => TypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));

            public ITypeDefinition[] TypeDefinition { get; }

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
        
        public static ITypeDefinition MethodType(ITypeDefinition input, ITypeDefinition output)
        {
            return new GenericFundimentalType("Method", input, output);
        }
        public static ITypeDefinition ImplementationType(ITypeDefinition context, ITypeDefinition input, ITypeDefinition output)
        {
            return new GenericFundimentalType("Implementation", context, input, output);
        }
    }

}