using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Semantic_Model.Operations
{
    public class ReturnOperation : CodeElement
    {
        public ReturnOperation(CodeElement result)
        {
            Result = result;
        }

        public CodeElement Result { get; }
    }

    public class Constant : CodeElement {
        public Constant(string value) {
            Value = value;
        }

        public string Value { get; }
    }
}
