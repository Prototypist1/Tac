using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public sealed partial class RootScope
    {
        private class FundimentalType : ITypeDefinition
        {
            public FundimentalType(string key)
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                Key = new ExplicitName(key);
            }

            public AbstractName Key { get; }

            public override bool Equals(object obj)
            {
                var type = obj as FundimentalType;
                return type != null &&
                       EqualityComparer<AbstractName>.Default.Equals(Key, type.Key);
            }

            public override int GetHashCode() => 990326508 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
            public ITypeDefinition ReturnType(ScopeStack scope) => this;
        }

        public static ITypeDefinition StringType { get; } = new FundimentalType("String");
        public static ITypeDefinition NumberType { get; } = new FundimentalType("Number");
        public static ITypeDefinition EmptyType { get; } = new FundimentalType("Empty");
        public static ITypeDefinition BooleanType { get; } = new FundimentalType("Boolean");
    }

}