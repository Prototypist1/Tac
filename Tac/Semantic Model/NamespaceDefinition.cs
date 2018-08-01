using System;
using System.Collections.Generic;
using Tac.Semantic_Model.Names;

namespace Tac.Semantic_Model
{

    public class NamespaceDefinition: AbstractScope, IReferanced<NamespaceName>
    {

        
        public NamespaceName Key { get; };

        public override TReferanced Get<TKey, TReferanced>(TKey key)
        {
            throw new NotImplementedException();
        }
    }

    
}