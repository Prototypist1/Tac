using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static partial class RootScope
    {

        public static IScope Root { get => Root; }
        private static StaticScope root = new StaticScope();

        public static AbstractName Add(TypeDefinition typeDefinition) {
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }

        public static AbstractName AddGeneric(GenericTypeDefinition typeDefinition)
        {
            if (!root.TryAddStaticGenericType(typeDefinition))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }
        
        public static AbstractName StringType { get; } = Add(new TypeDefinition(new ExplicitName("String"), new ObjectScope()));
        public static AbstractName NumberType { get; } = Add(new TypeDefinition(new ExplicitName("Number"), new ObjectScope()));
        public static AbstractName EmptyType { get; } = Add(new TypeDefinition(new ExplicitName("Empty"), new ObjectScope()));
        public static AbstractName AnyType { get; } = Add(new TypeDefinition(new ExplicitName("Any"), new ObjectScope()));
        public static AbstractName BooleanType { get; } = Add(new TypeDefinition(new ExplicitName("Bool"), new ObjectScope()));
        public static AbstractName TypeType { get; } = Add(new TypeDefinition(new ExplicitName("Type"), new ObjectScope()));

        public static AbstractName MethodType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitName("Method"),
                new ObjectScope(), 
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition(
                        new ExplicitName("Input")),
                    new GenericTypeParameterDefinition(
                        new ExplicitName("Output")) }));

        public static AbstractName ImplementationType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitName("Implementation"),
                new ObjectScope(),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition(
                        new ExplicitName("Context")),
                    new GenericTypeParameterDefinition(
                        new ExplicitName("Input")),
                    new GenericTypeParameterDefinition(
                        new ExplicitName("Output")) }));
    }

}