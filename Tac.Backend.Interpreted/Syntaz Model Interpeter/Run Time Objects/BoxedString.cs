using System;
using Tac.Model.Instantiated;

namespace Tac.Backend.Interpreted.SyntazModelInterpeter
{
    public interface IBoxedString: IInterpetedAnyType
    {
        string Value { get; }
    }

    public static partial class TypeManager
    {

        public static IBoxedString String(string value) => new RunTimeAnyRoot(new Func<IRunTimeAnyRoot, RunTimeAnyRootEntry>[] { StringIntention(value) }).Has<IBoxedString>();


        public static Func<IRunTimeAnyRoot, RunTimeAnyRootEntry> StringIntention(string value) => root => new RunTimeAnyRootEntry( new BoxedString(value, root),new StringType());

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