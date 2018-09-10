using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static partial class RootScope
    {


        //public class FundimentalType : ITypeDefinition
        //{
        //    public FundimentalType(string key)
        //    {
        //        if (key is null)
        //        {
        //            throw new ArgumentNullException(nameof(key));
        //        }
        //        Key = new ExplicitName(key);
        //    }

        //    public AbstractName Key { get; }

        //    public override bool Equals(object obj)
        //    {
        //        var type = obj as FundimentalType;
        //        return type != null &&
        //               EqualityComparer<AbstractName>.Default.Equals(Key, type.Key);
        //    }

        //    public override int GetHashCode() => 990326508 + EqualityComparer<AbstractName>.Default.GetHashCode(Key);
        //    public ITypeDefinition ReturnType(ScopeStack scope) => this;
        //}

        public static ITypeDefinition Add(TypeDefinition typeDefinition) {
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition;
        }
        
        public static ITypeDefinition StringType { get; } = Add(new TypeDefinition(new ExplicitName("String"), new ObjectScope()));
        public static ITypeDefinition NumberType { get; } = Add(new TypeDefinition(new ExplicitName("Number"), new ObjectScope()));
        public static ITypeDefinition EmptyType { get; } = Add(new TypeDefinition(new ExplicitName("Empty"), new ObjectScope()));
        public static ITypeDefinition AnyType { get; } = Add(new TypeDefinition(new ExplicitName("Any"), new ObjectScope()));
        public static ITypeDefinition BooleanType { get; } = Add(new TypeDefinition(new ExplicitName("Bool"), new ObjectScope()));
        public static ITypeDefinition TypeType { get; } = Add(new TypeDefinition(new ExplicitName("Type"), new ObjectScope()));
    }

}