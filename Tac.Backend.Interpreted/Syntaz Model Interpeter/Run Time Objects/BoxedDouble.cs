using System;
using Tac.Model.Instantiated;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{


    public interface IBoxedDouble: IInterpetedAnyType
    {
        double Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedDouble Double(double value)  => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { DoubleIntention(value) }).Has<IBoxedDouble>();
        
        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> DoubleIntention(double value) => root => new RunTimeAnyRootEntry( new BoxedDouble(value, root), new NumberType());

        private class BoxedDouble : RootedTypeAny, IBoxedDouble
        {
            public BoxedDouble(double value, IRunTimeAnyRoot root) : base(root)
            {
                Value = value;
            }

            public double Value { get; }
        }
    }
}