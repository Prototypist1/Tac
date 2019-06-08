using System;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IBoxedBool: IInterpetedAnyType
    {
        bool Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedBool Bool(bool value) => Root(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { BoolIntention(value) }).Has<IBoxedBool>();

        public static Func<IRunTimeAnyRoot, IBoxedBool> BoolIntention(bool value) => root => new BoxedBool(value,root);

        private class BoxedBool : RootedTypeAny, IBoxedBool
        {
            public BoxedBool(bool value, IRunTimeAnyRoot root) : base(root)
            {
                Value = value;
            }

            public bool Value { get; }
        }
    }
}