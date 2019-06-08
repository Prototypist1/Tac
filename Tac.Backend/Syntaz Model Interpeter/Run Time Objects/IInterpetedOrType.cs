using System;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedOrType<T1, T2> : IInterpetedAnyType
        where T1 : IInterpetedAnyType
        where T2 : IInterpetedAnyType
    {

    }


    public static partial class TypeManager
    {

        public static IInterpetedOrType<T1, T2> OrType<T1, T2>()
            where T1 : IInterpetedAnyType
            where T2 : IInterpetedAnyType 
            => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedOrType<T1, T2>>[] { OrTypeIntention<T1, T2>() }).Has<IInterpetedOrType<T1, T2>>();


        public static Func<IRunTimeAnyRoot, IInterpetedOrType<T1, T2>> OrTypeIntention<T1, T2>()

            where T1 : IInterpetedAnyType
            where T2 : IInterpetedAnyType 
            => root => new InterpedOrType<T1, T2>(root);

        private class InterpedOrType<T1, T2> : RootedTypeAny, IInterpetedOrType<T1, T2>
            where T1 : IInterpetedAnyType
            where T2 : IInterpetedAnyType
        {
            public InterpedOrType(IRunTimeAnyRoot root) : base(root)
            {
            }
        }
    }
}