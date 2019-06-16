using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model.Instantiated;

namespace Tac.Syntaz_Model_Interpeter { 

    public interface IInterpedEmpty : IInterpetedAnyType
    {
    }

    public static partial class TypeManager
    {

        public static IInterpedEmpty Empty() => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { EmptyIntention() }).Has<IInterpedEmpty>();


        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> EmptyIntention() => root => new RunTimeAnyRootEntry(new RunTimeEmpty(root), new EmptyType());

        private class RunTimeEmpty : RootedTypeAny, IInterpedEmpty
        {
            public RunTimeEmpty(IRunTimeAnyRoot root) : base(root)
            {
            }
        }

    }
}
