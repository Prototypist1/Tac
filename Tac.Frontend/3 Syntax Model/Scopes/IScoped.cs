using Prototypist.TaskChain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Model;
using Tac.Parser;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.Model.Elements;

namespace Tac.SemanticModel
{
    // I am not really sure this is a useful concept
    internal interface IScoped
    {
        IOrType<WeakScope, Tac.Model.IError> Scope { get; }
    }

    internal interface IFrontendGenericType : IFrontendType<IVerifiableType>
    {
        IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
    }

}

