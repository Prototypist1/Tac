using System;

namespace Tac.Syntaz_Model_Interpeter
{
    public interface IBoxedString: IInterpetedAnyType
    {
        string Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedString String(string value) => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, IInterpetedAnyType>[] { StringIntention(value) }).Has<IBoxedString>();


        public static Func<IRunTimeAnyRoot, IBoxedString> StringIntention(string value) => root => new BoxedString(value, root);

        private class BoxedString : RootedTypeAny, IBoxedString
        {
            public BoxedString(string value, IRunTimeAnyRoot root) : base(root)
            {
                Value = value ?? throw new System.ArgumentNullException(nameof(value));
            }

            public string Value { get; }

        }

    }
}