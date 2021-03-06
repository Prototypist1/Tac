﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Tac.Semantic_Model
{
    public interface IBox<out T> 
    {
        T GetValue();
    }


    public class Box<T> : IBox<T>
    {
        public Box()
        {
        }

        public Box(T innerType)
        {
            if (innerType == null)
            {
                throw new System.ArgumentNullException(nameof(innerType));
            }

            InnerType = innerType;
            HasThing = true;
        }

        private bool HasThing = false;
        private T InnerType { get; set; }

        public T GetValue()
        {
            return InnerType;
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