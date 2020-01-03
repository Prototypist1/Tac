using Prototypist.Toolbox.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.Frontend;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Model.Operations;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.CodeStuff;
using Tac.Semantic_Model.Operations;

namespace Tac.Semantic_Model.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public  static readonly string StaticNextCallSymbol = StaticSymbolsRegistry.AddOrThrow(">");
        public  readonly string NextCallSymbol = StaticNextCallSymbol;
        public static readonly string StaticLastCallSymbol = StaticSymbolsRegistry.AddOrThrow("<");
        public  readonly string LastCallSymbol = StaticLastCallSymbol;
    }

}


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticLastCallMaker = AddOperationMatcher(() => new LastCallOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> LastCallMaker = StaticLastCallMaker;
#pragma warning restore IDE0052 // Remove unread private members
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticNextCallMaker = AddOperationMatcher(() => new NextCallOperationMaker());
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> NextCallMaker = StaticNextCallMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{

    internal class WeakNextCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, INextCallOperation>
    {
        public WeakNextCallOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<INextCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = NextCallOperation.Create();
            return new BuildIntention<INextCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation,INextCallOperation>
    {
        public NextCallOperationMaker() : base(SymbolsRegistry.StaticNextCallSymbol, (l,r)=> new Box<WeakNextCallOperation>( new WeakNextCallOperation(l,r)),(s,c,l,r)=> {

            // nearly duplicate code 3930174039475
            // really this should not throw if the type requirement are not met
            // it is just a compliation error
            s.Problem.IsAssignedTo(l.SetUpSideNode.CastTo<Tpn.ICanAssignFromMe>(), r.SetUpSideNode.CastTo<Tpn.TypeProblem2.Method>().Input());
            return (r.SetUpSideNode as Tpn.TypeProblem2.Method).Returns(); 
        })
        {
        }
    }

    internal class WeakLastCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILastCallOperation>
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IBox<IFrontendCodeElement> left, IBox<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ILastCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = LastCallOperation.Create();
            return new BuildIntention<ILastCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetValue().ConvertElementOrThrow(context), Right.GetValue().ConvertElementOrThrow(context));
            });
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation, ILastCallOperation>
    {
        public LastCallOperationMaker() : base(SymbolsRegistry.StaticLastCallSymbol, (l,r)=>new Box<WeakLastCallOperation>( new WeakLastCallOperation(l,r)), (s, c, l, r) =>
        {
            // nearly duplicate code 3930174039475
            // really this should not throw if the type requirement are not met
            // it is just a compliation error
            s.Problem.IsAssignedTo(r.SetUpSideNode.CastTo<Tpn.ICanAssignFromMe>(), l.SetUpSideNode.CastTo<Tpn.TypeProblem2.Method>().Input()); ;
            return (l.SetUpSideNode as Tpn.TypeProblem2.Method).Returns();
        })
        {
        }
    }
}
