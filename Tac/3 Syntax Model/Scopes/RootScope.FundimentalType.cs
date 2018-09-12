using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static partial class RootScope
    {

        public static IScope Root { get => Root; }
        private static StaticScope root = new StaticScope();

        public static ITypeSource Add(TypeDefinition typeDefinition) {
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }

        public static ITypeSource AddGeneric(GenericTypeDefinition typeDefinition)
        {
            if (!root.TryAddStaticGenericType(typeDefinition))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }
        
        public static ITypeSource StringType { get; } = Add(new TypeDefinition(new ExplicitTypeName("String"), new ObjectScope()));
        public static ITypeSource NumberType { get; } = Add(new TypeDefinition(new ExplicitTypeName("Number"), new ObjectScope()));
        public static ITypeSource EmptyType { get; } = Add(new TypeDefinition(new ExplicitTypeName("Empty"), new ObjectScope()));
        public static ITypeSource AnyType { get; } = Add(new TypeDefinition(new ExplicitTypeName("Any"), new ObjectScope()));
        public static ITypeSource BooleanType { get; } = Add(new TypeDefinition(new ExplicitTypeName("Bool"), new ObjectScope()));
        public static ITypeSource TypeType { get; } = Add(new TypeDefinition(new ExplicitTypeName("Type"), new ObjectScope()));

        public static ITypeSource MethodType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitTypeName("Method"),
                new ObjectScope(), 
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("Input"),
                    new GenericTypeParameterDefinition("Output") }));

        public static ITypeSource ImplementationType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitTypeName("Implementation"),
                new ObjectScope(),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("Context"),
                    new GenericTypeParameterDefinition("Input"),
                    new GenericTypeParameterDefinition("Output") }));
    }

}