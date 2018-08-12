using System;
using Tac.Semantic_Model.CodeStuff;

namespace Tac.Parser
{
    public class Parser
    {

        public CodeElement Whatever(string value, Operations operations, ParseContext context)
        {
            if (operations.BinaryOperations.ContainsKey(value))
            {
                return operations.BinaryOperations[value](context.Last(), context.Next());
            }
            if (operations.NextOperations.ContainsKey(value))
            {
                return operations.NextOperations[value](context.Next());
            }
            if (operations.NextOperations.ContainsKey(value))
            {
                return operations.LastOperations[value](context.Last());
            }
            if (operations.ConstantOperations.ContainsKey(value))
            {
                return operations.ConstantOperations[value]();
            }
            return ParseElement(value);
        }
        
        public CodeElement ParseElement(string s) => throw new NotImplementedException();

    }

}
