using Prototypist.TaskChain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Parser;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;

namespace Tac.SemanticModel
{
    // I am not really sure this is a useful concept
    internal interface IScoped
    {
        IOrType<IBox<WeakScope>, IError> Scope { get; }
    }

    internal interface IFrontendGenericType : IFrontendType
    {
        IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
    }

}

