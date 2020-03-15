using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.SemanticModel
{
    public interface IBox<out T> 
    {
        T GetValue();
    }


    public class Box<T> : IBox<T>
        where T:class
    {
        public Box()
        {
        }

        public Box(T innerType)
        {
            InnerType = innerType ?? throw new System.ArgumentNullException(nameof(innerType));
            HasThing = true;
        }

        private bool HasThing = false;
        private T? InnerType { get; set; }

        public T GetValue()
        {
            return InnerType!;
        }

        public TT Fill<TT>(TT t)
            where TT : T
        {
            if (HasThing)
            {
                throw new Exception();
            }
            InnerType = t ?? throw new ArgumentNullException(nameof(t));
            HasThing = true;
            return t;
        }
    }
    
}