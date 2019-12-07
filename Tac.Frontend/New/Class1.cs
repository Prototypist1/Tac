using System;
using System.Collections.Generic;
using System.Text;
using Tac.Frontend;
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
        ISetUpTypeProblem TypeProblem { get; }
    }

    internal class SetUpContext : ISetUpContext
    {
        public ISetUpTypeProblem TypeProblem { get; }
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

    public interface IResolveContext
    {
    }

    public class ResolveContext : IResolveContext
    {
    }

    internal interface IResolve<out TCodeElement> 
    {
        IIsPossibly<TCodeElement> Run(IResolvableScope scope, IResolveContext context);
    }
    
}
