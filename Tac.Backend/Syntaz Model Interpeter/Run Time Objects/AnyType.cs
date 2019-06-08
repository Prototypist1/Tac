using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IInterpetedAnyType : IInterpeted {

        T Has<T>() where T : IInterpetedAnyType;

    }

    public interface IRunTimeAnyRoot: IInterpetedAnyType { }

    public static partial class TypeManager {

        // commented becuase it does not save much code and the cast stinks
        // pretty cheap code to duplicate 
        // T: class, stinks too
        // 😢
        //private static T CreateFromIntention<T>(Func<RunTimeAnyRoot, T> value)
        //    where T :  IInterpetedAnyType
        //{
        //    return new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { (x)=> value(x).Cast<IInterpetedAnyType>() }).Has<T>();
        //}


        internal abstract class RootedTypeAny : IInterpetedAnyType
        {
            private readonly IRunTimeAnyRoot root;

            protected RootedTypeAny(IRunTimeAnyRoot root)
            {
                this.root = root ?? throw new ArgumentNullException(nameof(root));
            }

            public T Has<T>() where T : IInterpetedAnyType => root.Has<T>();
        }

        public static IInterpetedAnyType AnyIntention(params Func<IRunTimeAnyRoot, IInterpetedAnyType>[] elements) => new RunTimeAnyRoot(elements);

        public static IRunTimeAnyRoot Root(IEnumerable<Func<IRunTimeAnyRoot, IInterpetedAnyType>> items) => new RunTimeAnyRoot(items);

        internal class RunTimeAnyRoot : IRunTimeAnyRoot
        {
            private readonly IReadOnlyList<IInterpetedAnyType> items;

            public RunTimeAnyRoot(IEnumerable<Func<IRunTimeAnyRoot, IInterpetedAnyType>> items)
            {
                this.items = items?.Select(x=>x(this)).ToList() ?? throw new ArgumentNullException(nameof(items));
                // todo assert that these are all of different types 
            }

            public T Has<T>() where T : IInterpetedAnyType => items.OfType<T>().Single();
        
        }       
    }
}
