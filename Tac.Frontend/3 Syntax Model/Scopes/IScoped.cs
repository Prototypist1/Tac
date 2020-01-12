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
        IBox<WeakScope> Scope { get; }
    }

    internal interface IFrontendGenericType : IFrontendType
    {
        IIsPossibly<IGenericTypeParameterPlacholder>[] TypeParameterDefinitions { get; }
        //OrType<IFrontendGenericType, IConvertableFrontendType<IVerifiableType>> Overlay(TypeParameter[] typeParameters);
    }

    internal class TypeParameter {
        public readonly IGenericTypeParameterPlacholder parameterDefinition;
        public readonly IFrontendType frontendType;

        public TypeParameter(IGenericTypeParameterPlacholder parameterDefinition, IFrontendType frontendType)
        {
            this.parameterDefinition = parameterDefinition;
            this.frontendType = frontendType ?? throw new ArgumentNullException(nameof(frontendType));
        }
    }
}

