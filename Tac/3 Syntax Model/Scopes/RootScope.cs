using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static class RootScope
    {

        public static StaticScope Root
        {
            get;
        } = new StaticScope();

        public static ExplicitTypeName Add(ExplicitTypeName name) {
            var typeDefinition = new NamedTypeDefinition(name.Key, new ObjectScope());
            if (!Root.TryAddStaticType(typeDefinition)) {
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
            if (!Root.TryAddStaticGenericType(typeDefinition))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return name;
        }

        // these need to move to IElementBuilder
        public static ExplicitTypeName StringType { get; } = Add(new ExplicitTypeName("string"));
        public static ExplicitTypeName NumberType { get; } = Add(new ExplicitTypeName("int"));
        public static ExplicitTypeName EmptyType { get; } = Add(new ExplicitTypeName("empty"));
        public static ExplicitTypeName AnyType { get; } = Add(new ExplicitTypeName("any"));
        public static ExplicitTypeName BooleanType { get; } = Add(new ExplicitTypeName("bool"));
        public static ExplicitTypeName TypeType { get; } = Add(new ExplicitTypeName("type"));

        public static ExplicitTypeName MethodType { get; } = AddGeneric(
                new ExplicitTypeName("method"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("input"),
                    new GenericTypeParameterDefinition("output") });

        public static ExplicitTypeName ImplementationType { get; } = AddGeneric(
                new ExplicitTypeName("implementation"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("context"),
                    new GenericTypeParameterDefinition("input"),
                    new GenericTypeParameterDefinition("output") });
    }

}