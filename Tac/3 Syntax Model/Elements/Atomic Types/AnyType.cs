using System;
using System.Collections.Generic;
using System.Text;
using Tac.Semantic_Model;

namespace Tac._3_Syntax_Model.Elements.Atomic_Types
{
    // todo is this really a thing?
    public class AnyType : IReturnable {
        public delegate AnyType Make();
    }
    
}
