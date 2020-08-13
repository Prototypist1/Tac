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
        IIsPossibly<ISetUpContext> Parent { get; }
        IIsPossibly<ISetUp> EnclosingSetUp { get; }
        Tpn.TypeProblem2.Builder TypeProblem { get; }

        ISetUpContext CreateChild(ISetUp setUp);
    }

    internal class SetUpContext : ISetUpContext
    {
        public SetUpContext(Tpn.TypeProblem2.Builder typeProblem) : this(typeProblem, Possibly.IsNot<ISetUpContext>(), Possibly.IsNot<ISetUp>()) { }

        private SetUpContext(Tpn.TypeProblem2.Builder typeProblem, IIsPossibly<ISetUpContext> parent, IIsPossibly<ISetUp> enclosingSetUp)
        {
            TypeProblem = typeProblem ?? throw new ArgumentNullException(nameof(typeProblem));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            EnclosingSetUp = enclosingSetUp ?? throw new ArgumentNullException(nameof(enclosingSetUp));
        }

        public Tpn.TypeProblem2.Builder TypeProblem { get; }

        public IIsPossibly<ISetUpContext> Parent
        {
            get;
        }

        public IIsPossibly<ISetUp> EnclosingSetUp
        {
            get;
        }

        public ISetUpContext CreateChild(ISetUp setUp)
        {
            return new SetUpContext(TypeProblem, Possibly.Is(this), Possibly.Is(setUp));
        }
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

    internal interface ISetUp { }

    internal interface ISetUp<out TCodeElement, out TSetUpSideNode>: ISetUp
        where TSetUpSideNode: Tpn.ITypeProblemNode
    {
        ISetUpResult<TCodeElement, TSetUpSideNode> Run(Tpn.IStaticScope scope, ISetUpContext context);
    }

    internal interface IResolve<out TCodeElement> 
    {
        TCodeElement Run(Tpn.TypeSolution context);
    }
    
}
