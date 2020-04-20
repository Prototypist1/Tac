using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : IConvertable
    {
        IKey Key { get; }
        IVerifiableType Type { get; }
        bool ReadOnly { get; }
    }
}