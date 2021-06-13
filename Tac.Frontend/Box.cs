using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.SemanticModel
{
    public interface IBox<out T> 
    {
        T GetValue();
    }

    public static class BoxEntensions {

        // I think this is the only correct way to get a funcBox
        // TODO make FuncBox an innder class and have this be the only thing that knows about it
        public static IBox<TT> Transfrom<T,TT>(this IBox<T> box, Func<T, TT> func)
        {
            return new FuncBox<TT>(() => func(box.GetValue()));
        }

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
            if (!HasThing) {
                throw new Exception("a box was opened before it was filled!");
            }

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

        public override string? ToString()
        {
            return $"{nameof(Box<T>)}({InnerType})";
        }
    }



    public class FuncBox<T> : IBox<T>
    {
        public FuncBox(Func<T> innerType)
        {
            InnerType = new Lazy<T>( innerType) ?? throw new System.ArgumentNullException(nameof(innerType));
        }

        private Lazy<T> InnerType { get; set; }

        public T GetValue()
        {
            return InnerType.Value;
        }
    }
}