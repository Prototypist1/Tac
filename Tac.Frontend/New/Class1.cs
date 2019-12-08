using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
using Tac.Frontend._3_Syntax_Model.Operations;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model.Elements;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;
using static Tac.SyntaxModel.Elements.AtomicTypes.PrimitiveTypes;

namespace Tac.New
{

    
    internal interface ISetUpContext
    {
        LocalTpn.ISetUpTypeProblem TypeProblem { get; }
    }

    internal class SetUpContext : ISetUpContext
    {
        public LocalTpn.ISetUpTypeProblem TypeProblem { get; }
    }

    internal interface ISetUpResult<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode : LocalTpn.ITypeProblemNode
    {
        IResolve<TCodeElement> Resolve { get; }
        TSetUpSideNode SetUpSideNode { get; }
    }

    internal struct SetUpResult<TCodeElement, TSetUpSideNode>: ISetUpResult<TCodeElement, TSetUpSideNode>
        where TSetUpSideNode : LocalTpn.ITypeProblemNode
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
        where TSetUpSideNode: LocalTpn.ITypeProblemNode
    {
        ISetUpResult<TCodeElement, TSetUpSideNode> Run(LocalTpn.IScope scope, ISetUpContext context);
    }

    internal interface IResolveContext
    {
        public LocalTpn.ITypeSolution Solution { get; }
    }

    internal class ResolveContext : IResolveContext
    {
        public ResolveContext(Tpn<WeakScope, WeakTypeDefinition, WeakObjectDefinition, WeakTypeOrOperation, WeakMethodDefinition, PlaceholderValue, WeakMemberDefinition, WeakTypeReference>.ITypeSolution solution)
        {
            Solution = solution ?? throw new ArgumentNullException(nameof(solution));
        }

        public LocalTpn.ITypeSolution Solution
        {
            get;
        }
    }

    internal interface IResolve<out TCodeElement> 
    {
        IIsPossibly<TCodeElement> Run(IResolveContext context);
    }
    
}
