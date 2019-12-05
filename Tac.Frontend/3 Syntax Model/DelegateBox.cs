using System;

namespace Tac.Semantic_Model
{
    // todo - should this cache?
    public class DelegateBox<T> : IBox<T> 
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
    
}