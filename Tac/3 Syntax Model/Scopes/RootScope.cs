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

        public static NameKey Add(NameKey name) {
            var typeDefinition = new TypeDefinition(new ObjectScope(),name);
            if (!Root.TryAddStaticType(name,new Box<ITypeDefinition>(typeDefinition))) {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return name;
        }

        public static Func<IKey[], GenericNameKey> AddGeneric(NameKey name, GenericTypeParameterDefinition[] paramters)
        {
            var typeDefinition = new GenericTypeDefinition(
                name,
                new ObjectScope(),
                paramters);
            if (!Root.TryAddStaticGenericType(name, new Box<GenericTypeDefinition>(typeDefinition)))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return (IKey[] keys) => new GenericNameKey(name,keys);
        }

        // these need to move to IElementBuilder
        public static NameKey StringType { get; } = Add(new NameKey("string"));
        public static NameKey NumberType { get; } = Add(new NameKey("int"));
        public static NameKey EmptyType { get; } = Add(new NameKey("empty"));
        public static NameKey AnyType { get; } = Add(new NameKey("any"));
        public static NameKey BooleanType { get; } = Add(new NameKey("bool"));
        
        // todo this is probably generic too
        public static NameKey TypeType { get; } = Add(new NameKey("system-compile-type"));
        public static NameKey BlockType { get; } = Add(new NameKey("system-compile-block"));
        public static NameKey ModuleType { get; } = Add(new NameKey("system-compile-module"));

        public static Func<IKey[], GenericNameKey> PathPartType { get; } = AddGeneric(
            new NameKey("path-part"),
            new GenericTypeParameterDefinition[] {
                        new GenericTypeParameterDefinition("type") });

        public static Func<IKey[], GenericNameKey> MemberType { get; } = AddGeneric(
                new NameKey("member"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("type") });

        public static readonly GenericTypeParameterDefinition methodOutput = new GenericTypeParameterDefinition("output");

        public static Func<IKey[], GenericNameKey> MethodType { get; } = AddGeneric(
                new NameKey("method"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("input"),
                    methodOutput });

        // TODO delegate with params keyword 
        public static Func<IKey[], GenericNameKey> ImplementationType { get; } = AddGeneric(
                new NameKey("implementation"),
                new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("context"),
                    new GenericTypeParameterDefinition("input"),
                    new GenericTypeParameterDefinition("output") });
    }

}