using System;
using System.Runtime.Serialization;

namespace Tac.Frontend.SyntaxModel.Scopes
{
    [Serializable]
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class Bug : Exception
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        public Bug()
        {
        }

        public Bug(string message) : base(message)
        {
        }

        public Bug(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Bug(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}