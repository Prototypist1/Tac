using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter { 

    public interface IInterpedEmpty : IInterpetedAnyType
    {
    }

    public static partial class TypeManager
    {

        public static IInterpedEmpty Empty() => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { EmptyIntention() }).Has<IInterpedEmpty>();


        public static Func<IRunTimeAnyRoot, IInterpedEmpty> EmptyIntention() => root => new RunTimeEmpty(root);

        private class RunTimeEmpty : RootedTypeAny, IInterpedEmpty
        {
            public RunTimeEmpty(IRunTimeAnyRoot root) : base(root)
            {
            }
        }

    }
}
