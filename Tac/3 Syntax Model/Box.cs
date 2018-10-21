using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Semantic_Model
{
    public interface IBox<out T> where T : class
    {
        T GetValue();
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

        public TT Fill<TT>(TT t)
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