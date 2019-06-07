using System;

namespace Tac.Syntaz_Model_Interpeter
{

    internal interface IBoxedBool: IInterpetedAnyType
    {
        bool Value { get; }
    }

    internal static partial class TypeManager
    {

        public static IBoxedBool Bool(bool value) => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { BoolIntention(value) }).Has<IBoxedBool>();

        public static Func<RunTimeAnyRoot, IBoxedBool> BoolIntention(bool value) => root => new BoxedBool(value,root);

        private class BoxedBool : RootedTypeAny, IBoxedBool
        {
            public BoxedBool(bool value, RunTimeAnyRoot root) : base(root)
            {
                Value = value;
            }

            public bool Value { get; }
        }
    }
}