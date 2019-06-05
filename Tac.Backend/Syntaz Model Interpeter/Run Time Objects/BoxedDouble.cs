using System;

namespace Tac.Syntaz_Model_Interpeter
{


    internal interface IBoxedDouble
    {
        double Value { get; }
    }

    internal static partial class TypeManager
    {
        public static Func<RunTimeAnyRoot, IBoxedDouble> DoubleIntention(double value) => root => new BoxedDouble(value, root);

        private class BoxedDouble : RootedTypeAny, IBoxedDouble
        {
            public BoxedDouble(double value, RunTimeAnyRoot root) : base(root)
            {
                Value = value;
            }

            public double Value { get; }
        }
    }
}