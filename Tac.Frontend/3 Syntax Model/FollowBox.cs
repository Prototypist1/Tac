using System;

namespace Tac.Semantic_Model
{
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
    
}