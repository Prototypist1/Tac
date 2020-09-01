using System;
using Tac.Model.Instantiated;

namespace Tac.Syntaz_Model_Interpeter
{

    public interface IBoxedBool: IInterpetedAnyType
    {
        bool Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedBool Bool(bool value) => Root(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { BoolIntention(value) }).Has<IBoxedBool>();

        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> BoolIntention(bool value) => root => new RunTimeAnyRootEntry(new BoxedBool(value,root), new BooleanType());

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