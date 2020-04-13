using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.SyntaxModel.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model.Elements;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.Model;

namespace Tac.Infastructure
{

    
    internal interface ISetUpContext
    {
        Tpn.ISetUpTypeProblem TypeProblem { get; }
    }

    internal class SetUpContext : ISetUpContext
    {
        public SetUpContext(Tpn.ISetUpTypeProblem typeProblem)
        {
            TypeProblem = typeProblem ?? throw new ArgumentNullException(nameof(typeProblem));
        }

        public Tpn.ISetUpTypeProblem TypeProblem { get; }
    }

    internal interface ISetUpResult<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode : Tpn.ITypeProblemNode
    {
        IResolve<TCodeElement> Resolve { get; }
        IOrType<TSetUpSideNode, IError> SetUpSideNode { get; }
    }

    internal struct SetUpResult<TCodeElement, TSetUpSideNode>: ISetUpResult<TCodeElement, TSetUpSideNode>
        where TSetUpSideNode : Tpn.ITypeProblemNode
    {
        public SetUpResult(IResolve<TCodeElement> populateBoxes, IOrType< TSetUpSideNode,IError> setUpSideNode)
        {
            Resolve = populateBoxes ?? throw new ArgumentNullException(nameof(populateBoxes));
            SetUpSideNode = setUpSideNode;
        }

        public IResolve<TCodeElement> Resolve { get; }
        public IOrType<TSetUpSideNode, IError> SetUpSideNode { get; }
    }

    internal interface ISetUp<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode: Tpn.ITypeProblemNode
    {
        ISetUpResult<TCodeElement, TSetUpSideNode> Run(Tpn.IScope scope, ISetUpContext context);
    }

    internal interface IResolve<out TCodeElement> 
    {
        TCodeElement Run(Tpn.ITypeSolution context);
    }
    
}
