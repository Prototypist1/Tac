using Tac.Backend.Syntaz_Model_Interpeter.Elements;
using Tac.Model.Elements;

namespace Tac.Backend.Syntaz_Model_Interpeter
{
    internal interface IExternalMethodSource
    {
        InterpetedExternalMethodDefinition GetExternalMethod(IExternalMethodDefinition codeElement);
    }

    internal class ExternalMethodSource : IExternalMethodSource
    {
        public InterpetedExternalMethodDefinition GetExternalMethod(IExternalMethodDefinition codeElement)
        {

        }
    }

}