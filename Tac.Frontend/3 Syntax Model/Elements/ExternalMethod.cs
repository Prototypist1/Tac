using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.SemanticModel;

namespace Tac.Frontend.SyntaxModel.Elements
{

    // uhhh.. this never used?
    // not really atleast
    internal interface IMethodDefinition: IFrontendType
    {
        IIsPossibly<IFrontendType> InputType { get; }
        IIsPossibly<IFrontendType> OutputType { get; }
        IIsPossibly<IBox<IIsPossibly<IWeakMemberDefinition>>> ParameterDefinition { get; }
    }
}
