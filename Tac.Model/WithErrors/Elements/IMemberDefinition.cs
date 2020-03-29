using Tac.Model.Elements;

namespace Tac.Model.WithErrors.Elements
{
    public interface IMemberDefinition : IConvertable
    {
        IKey Key { get; }
        IVerifiableType Type { get; }
        bool ReadOnly { get; }
    }
}