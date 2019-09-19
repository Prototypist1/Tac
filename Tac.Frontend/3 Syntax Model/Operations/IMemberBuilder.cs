//using System.Collections.Generic;
//using Tac.Frontend;
//using Tac.Model;

//namespace Tac.Semantic_Model.Operations
//{
//    //internal interface IMemberBuilder
//    //{
//    //    IKey Key { get; }
//    //    void AcceptsType(Box<IIsPossibly<IFrontendType>> box);
//    //    IBox<IIsPossibly<WeakMemberDefinition>> Build(); 
//    //}

//    internal class MemberBuilder : IMemberBuilder
//    {
//        public IKey Key { get; };

//        public MemberBuilder(IKey key) {
//            this.Key = key?? throw new System.ArgumentNullException(nameof(key));
//        }

//        private readonly List<Box<IIsPossibly<IFrontendType>>> accepts = new List<Box<IFrontendType>>();

//        public void AcceptsType(Box<IIsPossibly<IFrontendType>> box)
//        {
//            accepts.Add(box);
//        }

//        public IBox<IIsPossibly<WeakMemberDefinition>> Build()
//        {

//        }
//    }
//}