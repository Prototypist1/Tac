using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IBoxedString: IInterpetedAnyType
    {
        string Value { get; }
    }

    internal static partial class TypeManager
    {

        public static IBoxedString String(string value) => new RunTimeAnyRoot(new Func<RunTimeAnyRoot, IInterpetedAnyType>[] { StringIntention(value) }).Has<IBoxedString>();


        public static Func<RunTimeAnyRoot, IBoxedString> StringIntention(string value) => root => new BoxedString(value, root);

        private class BoxedString : RootedTypeAny, IBoxedString
        {
            public BoxedString(string value, RunTimeAnyRoot root) : base(root)
            {
                Value = value ?? throw new System.ArgumentNullException(nameof(value));
            }

            public string Value { get; }

        }

    }
}