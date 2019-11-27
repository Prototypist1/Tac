using System;
using System.Runtime.Serialization;

namespace Tac.Frontend._3_Syntax_Model.Scopes
{
    [Serializable]
    internal class Bug : Exception
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