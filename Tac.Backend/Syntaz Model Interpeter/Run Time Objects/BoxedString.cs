using System;

namespace Tac.Syntaz_Model_Interpeter
{
    internal interface IBoxedString
    {
        string Value { get; }
    }

    internal static partial class TypeManager
    {
        public static Func<RunTimeAnyRoot, IBoxedString> BoolIntention(string value) => root => new BoxedString(value, root);

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