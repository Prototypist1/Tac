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

namespace Tac.New
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
        TSetUpSideNode SetUpSideNode { get; }
    }

    internal struct SetUpResult<TCodeElement, TSetUpSideNode>: ISetUpResult<TCodeElement, TSetUpSideNode>
        where TSetUpSideNode : Tpn.ITypeProblemNode
    {
        public SetUpResult(IResolve<TCodeElement> populateBoxes, TSetUpSideNode setUpSideNode)
        {
            Resolve = populateBoxes ?? throw new ArgumentNullException(nameof(populateBoxes));
            SetUpSideNode = setUpSideNode;
        }

        public IResolve<TCodeElement> Resolve { get; }
        public TSetUpSideNode SetUpSideNode { get; }
    }

    internal interface ISetUp<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode: Tpn.ITypeProblemNode
    {
        ISetUpResult<TCodeElement, TSetUpSideNode> Run(Tpn.IScope scope, ISetUpContext context);
    }

    //internal interface IResolveContext
    //{
    //    public Tpn.ITypeSolution Solution { get; }
    //}

    //internal class ResolveContext : IResolveContext
    //{
    //    public ResolveContext(Tpn<WeakBlockDefinition, OrType<WeakTypeDefinition, WeakGenericTypeDefinition>, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition,  WeakTypeReference>.ITypeSolution solution)
    //    {
    //        Solution = solution ?? throw new ArgumentNullException(nameof(solution));
    //    }

    //    public Tpn.ITypeSolution Solution
    //    {
    //        get;
    //    }
    //}

    internal interface IResolve<out TCodeElement> 
    {
        IBox<TCodeElement> Run(Tpn.ITypeSolution context);
    }
    
}
