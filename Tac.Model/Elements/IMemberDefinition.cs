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
        ReadOnly=1,
        ReadWrite=3,
        WriteOnly=2
    }
}