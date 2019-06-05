using System;
using System.Collections.Generic;
using System.Text;

namespace Tac.Syntaz_Model_Interpeter { 

    internal interface IInterpedEmpty : IInterpetedAnyType
    {
    }

    internal static partial class TypeManager
    {
        public static Func<RunTimeAnyRoot, IInterpedEmpty> EmptyIntention() => root => new RunTimeEmpty(root);

        private class RunTimeEmpty : RootedTypeAny, IInterpedEmpty
        {
            public RunTimeEmpty(RunTimeAnyRoot root) : base(root)
            {
            }
        }

    }
}
