//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Tac.Semantic_Model.Names
//{
//    // why does this exist???
//    // . is an operation! 
//    // it can't be for types tho...
//    // what a mess! 
//    // TODO 
//    // TODO
//    public class NamePath {
//        public readonly AbstractName[] names;

//        public NamePath(AbstractName[] names) => this.names = names ?? throw new ArgumentNullException(nameof(names));

//        public override bool Equals(object obj)
//        {
//            return obj is NamePath path &&
//                   names.SequenceEqual(path.names);
//        }

//        public override int GetHashCode() => 149859047 + names.Sum(x=>x.GetHashCode());
//    }
////}
