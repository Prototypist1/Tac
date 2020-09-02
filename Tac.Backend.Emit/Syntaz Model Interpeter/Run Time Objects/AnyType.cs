using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Backend.Emit.SyntaxModel.Run_Time_Objects;

namespace Tac.Backend.Emit.SyntaxModel
{
    public interface IInterpetedAnyType : IInterpeted {

        T Has<T>() where T : IInterpetedAnyType;
        IVerifiableType Convert(IConversionContext conversionContext);
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
        //    return new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { (x)=> value(x).CastTo<IInterpetedAnyType>() }).Has<T>();
        //}


        internal abstract class RootedTypeAny : IInterpetedAnyType
        {
            private readonly IRunTimeAnyRoot root;

            protected RootedTypeAny(IRunTimeAnyRoot root)
            {
                this.root = root ?? throw new ArgumentNullException(nameof(root));
            }


            public IVerifiableType Convert(IConversionContext conversionContext)
            {
                return root.Convert(conversionContext);
            }

            public T Has<T>() where T : IInterpetedAnyType => root.Has<T>();

        }

        public static IInterpetedAnyType AnyIntention(params Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] elements) => new RunTimeAnyRoot(elements);

        public static IRunTimeAnyRoot Root(IEnumerable<Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>> items) => new RunTimeAnyRoot(items);

        public class RunTimeAnyRootEntry {
            public RunTimeAnyRootEntry(IInterpetedAnyType interpetedType,IVerifiableType compileTimeType)
            {
                InterpetedType = interpetedType ?? throw new ArgumentNullException(nameof(interpetedType));
                CompileTimeType = compileTimeType ?? throw new ArgumentNullException(nameof(compileTimeType));
            }

            public IInterpetedAnyType InterpetedType { get; }
            public IVerifiableType CompileTimeType { get; }
        }

        internal class RunTimeAnyRoot : IRunTimeAnyRoot
        {
            private readonly IReadOnlyList<IInterpetedAnyType> items;
            private readonly Func<IConversionContext, IVerifiableType> convertType;

            public RunTimeAnyRoot(IEnumerable<Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>> items)
            {
                var executedItems = items?.Select(x => x(this)).ToList() ?? throw new ArgumentNullException(nameof(items));

                convertType = GetType(executedItems);

                this.items = executedItems.Select(x => x.InterpetedType).ToList();
                // todo assert that these are all of different types 
            }

            private Func<IConversionContext, IVerifiableType> GetType(IReadOnlyList<RunTimeAnyRootEntry> executedItems) {
                return (IConversionContext context) =>
                {
                    if (!executedItems.Any())
                    {
                        throw new Exception("should be something in here...");
                    }
                    else if (executedItems.Count() == 1)
                    {
                        return executedItems.Single().CompileTimeType;
                    }
                    else
                    {
                        var myType = new TypeAnd(executedItems[0].CompileTimeType, executedItems[1].CompileTimeType);
                        var at = 2;
                        for (; at < executedItems.Count; at++)
                        {
                            myType = new TypeAnd(myType, executedItems[at].CompileTimeType);
                        }
                        return myType;
                    }
                };
            }

            public IVerifiableType Convert(IConversionContext conversionContext) => convertType(conversionContext);

            public T Has<T>() where T : IInterpetedAnyType => items.OfType<T>().Single();
        
        }       
    }
}
