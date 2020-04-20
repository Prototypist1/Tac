using Prototypist.Toolbox;
using System;
using Tac.Model.Elements;

namespace Tac.Model.Instantiated
{
    public class MemberDefinition : IMemberDefinition, IMemberDefinitionBuilder
    {
        private MemberDefinition() { }

        private readonly Buildable<IKey> buildableKey = new Buildable<IKey>();
        private readonly Buildable<IVerifiableType> buildableType = new Buildable<IVerifiableType>();
        private readonly BuildableValue<bool> buildableReadOnly = new BuildableValue<bool>();
        
        public IKey Key { get => buildableKey.Get(); }
        public IVerifiableType Type { get => buildableType.Get(); }
        public bool ReadOnly { get => buildableReadOnly.Get(); }

        public T Convert<T, TBacking>(IOpenBoxesContext<T, TBacking> context)
            where TBacking : IBacking
        {
            return context.MemberDefinition(this);
        }

        public void Build(IKey key, IVerifiableType type, bool readOnly)
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

        public static IMemberDefinition CreateAndBuild(IKey key, IVerifiableType type, bool readOnly)
        {
            var (x, y) = Create();
            y.Build(key, type, readOnly);
            return x;
        }

    }

    public interface IMemberDefinitionBuilder
    {
        void Build(IKey key, IVerifiableType type, bool readOnly);
    }
}