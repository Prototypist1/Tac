using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Semantic_Model
{
    public interface IBox<out T> where T : class
    {
        T GetValue();
    }

    // TODO unused?
    public class GenericBox : IBox<IReturnable>
    {
        private IBox<IGenericTypeDefinition> definition;
        private readonly IEnumerable<IBox<IReturnable>> genericTypeParameters;

        public GenericBox(IBox<IGenericTypeDefinition> definition, IEnumerable<IBox<IReturnable>> genericTypeParameters)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.genericTypeParameters = genericTypeParameters ?? throw new ArgumentNullException(nameof(genericTypeParameters));
        }

        public IReturnable GetValue()
        {
            var genericType = definition.GetValue();
            if (genericType.TryCreateConcrete(genericType.TypeParameterDefinitions.Zip(genericTypeParameters, (x, y) => new GenericTypeParameter(y, x)), out var box))
            {
                return box;
            }
            throw new Exception("whatever whatever your code is a pile of shit 💩💥");
        }
    }

    public class DelegateBox<T> : IBox<T> where T : class
    {
        private Func<T> func;

        public DelegateBox()
        {
        }

        public DelegateBox<T> Set(Func<T> func)
        {
            if (func == null)
            {
                throw new Exception("func already set");
            }
            this.func = func ?? throw new ArgumentNullException(nameof(func));
            return this;
        }

        public DelegateBox(Func<T> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public T GetValue()
        {
            return func();
        }
    }

    public class FollowBox<T> : IBox<T> where T : class
    {
        public FollowBox()
        {
        }

        private IBox<T> InnerType { get; set; }

        public IBox<T> Follow(IBox<T> box)
        {
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
}