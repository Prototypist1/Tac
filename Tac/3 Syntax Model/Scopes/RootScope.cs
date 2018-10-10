using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{


    public static class RootKeys {
        public static NameKey StringType { get; } = new NameKey("string");
        public static NameKey NumberType { get; } = new NameKey("int");
        public static NameKey EmptyType { get; } = new NameKey("empty");
        public static NameKey AnyType { get; } = new NameKey("any");
        public static NameKey BooleanType { get; } = new NameKey("bool");

        // todo this is probably generic too
        public static NameKey TypeType { get; } = new NameKey("system-compile-type");
        public static NameKey BlockType { get; } = new NameKey("system-compile-block");
        public static NameKey ModuleType { get; } = new NameKey("system-compile-module");

        public static NameKey PathPartType { get; }

        public static NameKey MemberType { get; } = new NameKey("member");

        public static NameKey MethodType { get; } = new NameKey("method");
    }

    public class RootScope
    {
        // these need to move to IElementBuilder
        public IBox<ITypeDefinition> StringType { get; }
        public IBox<ITypeDefinition> NumberType { get; }
        public IBox<ITypeDefinition> EmptyType { get; }
        public IBox<ITypeDefinition> AnyType { get; }
        public IBox<ITypeDefinition> BooleanType { get; }

        // todo this is probably generic too
        public IBox<ITypeDefinition> TypeType { get; }
        public IBox<ITypeDefinition> BlockType { get; }
        public IBox<ITypeDefinition> ModuleType { get; }

        public GetGenericType PathPartType { get; }
        public GetGenericType MemberType { get; }
        public GetGenericType MethodType { get; }



        //public Func<IKey[], GenericNameKey> ImplementationType { get; }

        public RootScope()
        {
            StringType = Add(RootKeys.StringType);
            NumberType = Add(RootKeys.NumberType);
            EmptyType = Add(RootKeys.EmptyType);
            AnyType = Add(RootKeys.AnyType);
            BooleanType = Add(RootKeys.BooleanType);

            TypeType = Add(RootKeys.TypeType);
            BlockType = Add(RootKeys.BlockType);
            ModuleType = Add(RootKeys.ModuleType);

            PathPartType = AddGeneric(
                 RootKeys.PathPartType,
                 new GenericTypeParameterDefinition[] {
                        new GenericTypeParameterDefinition("type") });

            MemberType = AddGeneric(
                    RootKeys.MemberType,
                    new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("type") });
           
            MethodType = AddGeneric(
                    RootKeys.MethodType,
                    new GenericTypeParameterDefinition[] {
                    new GenericTypeParameterDefinition("input"),
                    new GenericTypeParameterDefinition("output") });
        }


        private Scope Root
        {
            get;
        } = new Scope();

        public IBox<ITypeDefinition> Add(NameKey name)
        {
            var typeDefinition = new TypeDefinition(new Scope(), name);
            if (!Root.TryAddStaticType(name, new Box<ITypeDefinition>(typeDefinition)))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }
            return new Box<ITypeDefinition>(typeDefinition);
        }

        public delegate IBox<ITypeDefinition> GetGenericType(params IKey[] keys);

        public GetGenericType AddGeneric(NameKey name, GenericTypeParameterDefinition[] paramters)
        {
            var typeDefinition = new GenericTypeDefinition(
                name,
                new Scope(),
                paramters);
            if (!Root.TryAddStaticGenericType(name, new Box<GenericTypeDefinition>(typeDefinition)))
            {
                throw new Exception($"could not add type {typeDefinition}");
            }

            IBox<ITypeDefinition> GetGenericType(params IKey[] keys) {

                var key = new GenericNameKey(name, keys);
                if (Root.TryGetType(key, out var res)){
                    return res;
                }
                throw new Exception();
            }

            return GetGenericType;
        }
    }
}