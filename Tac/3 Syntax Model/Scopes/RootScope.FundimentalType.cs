using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static partial class RootScope
    {

        public static IScope Root { get => Root; }
        private static StaticScope root = new StaticScope();

        public static ExplicitTypeName Add(ExplicitTypeName name) {
            var typeDefinition = new NamedTypeDefinition(name.Key, new ObjectScope());
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return name;
        }

        public static ExplicitTypeName AddGeneric(ExplicitTypeName name, GenericTypeParameterDefinition[] paramters)
        {
            var typeDefinition = new GenericTypeDefinition(
                name.Key,
                new ObjectScope(),
                paramters);
            if (!root.TryAddStaticGenericType(typeDefinition))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return name;
        }
        
        public static ExplicitTypeName StringType { get; } = Add(new ExplicitTypeName("String"));
        public static ExplicitTypeName NumberType { get; } = Add(new ExplicitTypeName("Number"));
        public static ExplicitTypeName EmptyType { get; } = Add(new ExplicitTypeName("Empty"));
        public static ExplicitTypeName AnyType { get; } = Add(new ExplicitTypeName("Any"));
        public static ExplicitTypeName BooleanType { get; } = Add(new ExplicitTypeName("Bool"));
        public static ExplicitTypeName TypeType { get; } = Add(new ExplicitTypeName("Type"));

        public static ExplicitTypeName MethodType { get; } = AddGeneric(
                new ExplicitTypeName("Method"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("Input"),
                    new GenericTypeParameterDefinition("Output") });

        public static ExplicitTypeName ImplementationType { get; } = AddGeneric(
                new ExplicitTypeName("Implementation"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("Context"),
                    new GenericTypeParameterDefinition("Input"),
                    new GenericTypeParameterDefinition("Output") });
    }

}