using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : IConvertable
    {
        IKey Key { get; }
        IVerifiableType Type { get; }
        Access Access { get; }
    }

    public enum Access { 
        ReadOnly,
        ReadWrite,
        WriteOnly
    }
}