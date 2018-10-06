using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{
    public interface IBox<out T> where T : class
    {
        T GetValue();
    }

    public class GenericBox : IBox<ITypeDefinition>
    {
        private IBox<IGenericTypeDefinition> definition;
        private readonly IEnumerable<IBox<ITypeDefinition>> genericTypeParameters;

        public GenericBox(IBox<IGenericTypeDefinition> definition, IEnumerable<IBox<ITypeDefinition>> genericTypeParameters)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.genericTypeParameters = genericTypeParameters ?? throw new ArgumentNullException(nameof(genericTypeParameters));
        }

        public ITypeDefinition GetValue()
        {
            var genericType = definition.GetValue();
            if (genericType.TryCreateConcrete(genericType.TypeParameterDefinitions.Zip(genericTypeParameters, (x, y) => new GenericTypeParameter(y, x)), out var box)) {
                return box;
            }
            throw new Exception("whatever whatever your code is a pile of shit 💩💥");
        }
    }
    
    public class PathBox : IBox<MemberDefinition>
    {
        private IBox<MemberDefinition> inner;

        public NameKey Key { get; }

        public PathBox(NameKey memberKey)
        {
            this.Key = memberKey ?? throw new ArgumentNullException(nameof(memberKey));
        }


        public PathBox Follow(IBox<MemberDefinition> box)
        {
            if (inner != null)
            {
                throw new Exception();
            }
            inner = box;
            return this;
        }

        public MemberDefinition GetValue()
        {
            if (inner.GetValue().Cast<IScope>().TryGetMember(Key, false, out var res)) {
                return res.GetValue() ;
            }
            throw new Exception("this code will not complie, the object does not have a member");
        }
    }

    public class FollowBox<T> : IBox<T> where T : class {
        public FollowBox()
        {
        }

        private IBox<T> InnerType { get; set; }

        public IBox<T> Follow(IBox<T> box) {
            if (InnerType != null)
            {
                throw new Exception();
            }
            InnerType = box;
            return this;
        }

        public T GetValue()
        {
            return InnerType.GetValue();
        }
    }

    public class Box<T> : IBox<T> where T : class
    {
        public Box()
        {
        }

        public Box(T innerType)
        {
            InnerType = innerType ?? throw new System.ArgumentNullException(nameof(innerType));
        }

        private T InnerType { get; set; }

        public T GetValue()
        {
            return InnerType;
        }

        internal TT Fill<TT>(TT t)
            where TT : class, T
        {
            if (InnerType != null) {
                throw new Exception();
            }
            InnerType = t ?? throw new ArgumentNullException(nameof(t));
            return t;
        }
    }

    public interface IPopulatableScope {
        IResolvableScope ToResolvable();
    }

    public interface IResolvableScope {
        bool TryGetType(IKey name, out IBox<ITypeDefinition> type);
        bool TryGetMember(NameKey name, bool staticOnly, out IBox<MemberDefinition> member);
    }
    
    //public interface IScope
    //{
        //IReadOnlyList<IBox<MemberDefinition>> Members { get; }
        //IReadOnlyList<IBox<ITypeDefinition>> Types { get; }
    //}

    public static class IIResolvableScopeExtension
    {
        public static IBox<ITypeDefinition> GetTypeOrThrow(this IResolvableScope scope, NameKey name) {
            if (scope.TryGetType(name, out var thing)) {
                return thing;
            }
            throw new Exception($"{name} should exist in scope");
        }
    }
}