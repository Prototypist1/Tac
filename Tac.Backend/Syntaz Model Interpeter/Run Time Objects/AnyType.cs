using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Syntaz_Model_Interpeter.Run_Time_Objects;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IInterpetedAnyType : IInterpeted {

        T Has<T>() where T : IInterpetedAnyType;

    }

    internal static partial class TypeManager { 

        internal abstract class RootedTypeAny : IInterpetedAnyType
        {
            private readonly RunTimeAnyRoot root;

            protected RootedTypeAny(RunTimeAnyRoot root)
            {
                this.root = root ?? throw new ArgumentNullException(nameof(root));
            }

            public T Has<T>() where T : IInterpetedAnyType => root.Has<T>();
        }

        public static IInterpetedAnyType Create(params Func<RunTimeAnyRoot, IInterpetedAnyType>[] elements) => new RunTimeAnyRoot(elements);

        internal class RunTimeAnyRoot : IInterpetedAnyType
        {
            private readonly IReadOnlyList<IInterpetedAnyType> items;

            public RunTimeAnyRoot(IEnumerable<Func<RunTimeAnyRoot,IInterpetedAnyType>> items)
            {
                this.items = items?.Select(x=>x(this)).ToList() ?? throw new ArgumentNullException(nameof(items));
                // todo assert that these are all of different types 
            }

            public T Has<T>() where T : IInterpetedAnyType => items.OfType<T>().Single();
        
        }       
    }
}
