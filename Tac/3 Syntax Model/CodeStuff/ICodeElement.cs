﻿namespace Tac.Semantic_Model.CodeStuff
{
    public interface ICodeElement {
        ITypeDefinition ReturnType(ScopeStack scope);
    }
}