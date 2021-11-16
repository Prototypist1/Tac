using Prototypist.Toolbox;
using System;
using System.Threading;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class MemberDefinition : IMemberDefinition, IMemberDefinitionBuilder
    {
        private static int index = 0;
        private readonly int myIndex;
        private MemberDefinition() {
            myIndex = Interlocked.Increment(ref index);
        }

        private readonly Buildable<IKey> buildableKey = new Buildable<IKey>();
        private readonly Buildable<IVerifiableType> buildableType = new Buildable<IVerifiableType>();
        private readonly BuildableValue<Access> buildableReadOnly = new BuildableValue<Access>();
        
        public IKey Key { get => buildableKey.Get(); }
        public IVerifiableType Type { get => buildableType.Get(); }
        public Access Access { get => buildableReadOnly.Get(); }

        public T Convert<T>(IOpenBoxesContext<T> context)
        {
            return context.MemberDefinition(this);
        }

        public void Build(IKey key, IVerifiableType type, Access readOnly)
        {
            buildableKey.Set(key);
            buildableType.Set(type);
            buildableReadOnly.Set(readOnly);
        }

        public static (IMemberDefinition, IMemberDefinitionBuilder) Create()
        {
            var res = new MemberDefinition();
            return (res, res);
        }

        public static IMemberDefinition CreateAndBuild(IKey key, IVerifiableType type, Access readOnly)
        {
            var (x, y) = Create();
            y.Build(key, type, readOnly);
            return x;
        }

        public override string? ToString()
        {
            return $"{nameof(MemberDefinition)}-{myIndex}({Key})";
        }
    }

    public interface IMemberDefinitionBuilder
    {
        void Build(IKey key, IVerifiableType type, Access readOnly);
    }
}