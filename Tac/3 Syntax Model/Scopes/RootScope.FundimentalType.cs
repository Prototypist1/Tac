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

        public static ITypeDefinition<IScope> Add(TypeDefinition typeDefinition) {
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition;
        }
        
        public static ITypeDefinition<IScope> StringType { get; } = Add(new TypeDefinition(new ExplicitName("String")));
        public static ITypeDefinition<IScope> NumberType { get; } = Add(new TypeDefinition(new ExplicitName("Number")));
        public static ITypeDefinition<IScope> EmptyType { get; } = Add(new TypeDefinition(new ExplicitName("Empty")));
        public static ITypeDefinition<IScope> AnyType { get; } = Add(new TypeDefinition(new ExplicitName("Any")));
        public static ITypeDefinition<IScope> BooleanType { get; } = Add(new TypeDefinition(new ExplicitName("Bool")));
        public static ITypeDefinition<IScope> TypeType { get; } = Add(new TypeDefinition(new ExplicitName("Type")));

    }

}