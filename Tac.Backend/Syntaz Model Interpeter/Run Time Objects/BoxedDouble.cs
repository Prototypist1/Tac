using System;

namespace Tac.Syntaz_Model_Interpeter
{


    public interface IBoxedDouble: IInterpetedAnyType
    {
        double Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedDouble Double(double value)  => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { DoubleIntention(value) }).Has<IBoxedDouble>();
        
        public static Func<IRunTimeAnyRoot, IBoxedDouble> DoubleIntention(double value) => root => new BoxedDouble(value, root);

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