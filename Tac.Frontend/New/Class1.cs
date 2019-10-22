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
using static Tac._3_Syntax_Model.Elements.Atomic_Types.PrimitiveTypes;

namespace Tac.New
{

    
    internal interface IPopulateScopeContext
    {
        ISetUpTypeProblem TypeProblem { get; }
    }

    internal class PopulateScopeContext : IPopulateScopeContext
    {
        public ISetUpTypeProblem TypeProblem { get; }
    }

    internal interface IPopulateScope<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode: Tpn.ITypeProblemNode
    {
        IResolvelizeScope<TCodeElement, TSetUpSideNode> Run(Tpn.IScope scope, IPopulateScopeContext context);
    }

    internal interface IFinalizeScopeContext { }
    internal class FinalizeScopeContext: IFinalizeScopeContext { }


    internal interface IResolvelizeScope<out TCodeElement, out TSetUpSideNode>
        where TSetUpSideNode: Tpn.ITypeProblemNode
    {
        TSetUpSideNode SetUpSideNode { get; }

        // having this take a IResolvableScope is a little wierd
        // I don't want anything resolved until next time
        // I can't think of any thing that would break if you tried to resolve something here..
        // maybe these last two steps (IFinalizeScope and IPopulateBoxes) are really one step??
        IPopulateBoxes<TCodeElement> Run(IResolvableScope parent, IFinalizeScopeContext context);
    }

    public interface IResolveReferenceContext
    {
    }

    public class ResolveReferanceContext : IResolveReferenceContext
    {
    }

    internal interface IPopulateBoxes<out TCodeElement> 
    {
        IIsPossibly<TCodeElement> Run(IResolvableScope scope, IResolveReferenceContext context);
    }
    
}
