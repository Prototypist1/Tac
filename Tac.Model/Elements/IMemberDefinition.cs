using Tac.Model.Elements;

namespace Tac.Model.Elements
{
    public interface IMemberDefinition : IConvertable
    {
        IKey Key { get; }
        IOrType<IVerifiableType, IError> Type { get; }
        bool ReadOnly { get; }
    }
}