//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Tac.Model.Elements;

//namespace Tac.Backent.Emit._2
//{

//    public class TacObject
//    {
//        public IVerifiableType TacType;

//        public Func<object>[] Getters;
//        public Action<object>[] Setters;
//    }


//    public static class Help{
//        public static int GetIndex(IMemberDefinition memberDefinition, IInterfaceType verifiableType) {
//            return verifiableType.Members.IndexOf(x => x.Equals(memberDefinition));
//        }

//        public static int GetIndex(IMemberDefinition memberDefinition, ITypeOr verifiableType)
//        {
//            return verifiableType.Members.IndexOf(x => x.Equals(memberDefinition));
//        }

//        public static int IndexOf<T>(this IReadOnlyList<T> self, Func<T, bool> predicate)
//        {
//            for (int i = 0; i < self.Count; i++)
//            {
//                if (predicate(self[i]))
//                    return i;
//            }

//            return -1;
//        }
//    }
//}


/// ok
/// so 
/// every type/object is a real interface
/// when an object is assigned to a type we JIT a new class that implement that interface
/// this class as a pointer to the object assigned from and all it's properties pass through
/// if a JIT-ed object is assigned to another type we notice and don't layer, they use the same backing
/// we keep track of CLR types to the object->type conversion they represent so:
///     1 - we don't make doubles
///     2 - we can get the IVerifiableType from the CLR type for the 'is' operator and the like
///     
///
