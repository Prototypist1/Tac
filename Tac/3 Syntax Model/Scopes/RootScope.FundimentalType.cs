using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public static partial class RootScope
    {

        public static IScope Root { get => Root; }
        private static StaticScope root = new StaticScope();

        public static AbstractMemberName Add(TypeDefinition typeDefinition) {
            if (!root.TryAddStaticType(typeDefinition)) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }

        public static AbstractMemberName AddGeneric(GenericTypeDefinition typeDefinition)
        {
            if (!root.TryAddStaticGenericType(typeDefinition))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return typeDefinition.Key;
        }
        
        public static AbstractMemberName StringType { get; } = Add(new TypeDefinition(new ExplicitMemberName("String"), new ObjectScope()));
        public static AbstractMemberName NumberType { get; } = Add(new TypeDefinition(new ExplicitMemberName("Number"), new ObjectScope()));
        public static AbstractMemberName EmptyType { get; } = Add(new TypeDefinition(new ExplicitMemberName("Empty"), new ObjectScope()));
        public static AbstractMemberName AnyType { get; } = Add(new TypeDefinition(new ExplicitMemberName("Any"), new ObjectScope()));
        public static AbstractMemberName BooleanType { get; } = Add(new TypeDefinition(new ExplicitMemberName("Bool"), new ObjectScope()));
        public static AbstractMemberName TypeType { get; } = Add(new TypeDefinition(new ExplicitMemberName("Type"), new ObjectScope()));

        public static AbstractMemberName MethodType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitMemberName("Method"),
                new ObjectScope(), 
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition(
                        new ExplicitMemberName("Input")),
                    new GenericTypeParameterDefinition(
                        new ExplicitMemberName("Output")) }));

        public static AbstractMemberName ImplementationType { get; } = AddGeneric(
            new GenericTypeDefinition(
                new ExplicitMemberName("Implementation"),
                new ObjectScope(),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition(
                        new ExplicitMemberName("Context")),
                    new GenericTypeParameterDefinition(
                        new ExplicitMemberName("Input")),
                    new GenericTypeParameterDefinition(
                        new ExplicitMemberName("Output")) }));
    }

}