using Prototypist.LeftToRight;
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
        public WeakNextCallOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Right.GetOrThrow().Unwrap<Frontend._3_Syntax_Model.Elements.IMethodDefinition>().OutputType
                .IfIs(x => x.TypeDefinition)
                .IfIs(x => x.GetValue());
        }

        public override IBuildIntention<INextCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = NextCallOperation.Create();
            return new BuildIntention<INextCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation,INextCallOperation>
    {
        public NextCallOperationMaker() : base(SymbolsRegistry.StaticNextCallSymbol, (l,r)=> Possibly.Is( new WeakNextCallOperation(l,r)),(s,c,l,r)=> {

            (l.SetUpSideNode as Tpn.IValue).AssignTo((r.SetUpSideNode as Tpn.IMethod).Input());
            return (r.SetUpSideNode as Tpn.IMethod).Returns(); })
        {
        }
    }

    internal class WeakLastCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILastCallOperation>
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IIsPossibly<IFrontendCodeElement> left, IIsPossibly<IFrontendCodeElement> right) : base(left, right)
        {
        }
        
        public override IIsPossibly<IFrontendType> Returns()
        {
            return Left.GetOrThrow().Unwrap<Frontend._3_Syntax_Model.Elements.IMethodDefinition>().OutputType
                .IfIs(x=>x.TypeDefinition)
                .IfIs(x=>x.GetValue());
        }

        public override IBuildIntention<ILastCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = LastCallOperation.Create();
            return new BuildIntention<ILastCallOperation>(toBuild, () =>
            {
                maker.Build(Left.GetOrThrow().ConvertElementOrThrow(context), Right.GetOrThrow().ConvertElementOrThrow(context));
            });
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation, ILastCallOperation>
    {
        public LastCallOperationMaker() : base(SymbolsRegistry.StaticLastCallSymbol, (l,r)=>Possibly.Is( new WeakLastCallOperation(l,r)), (s, c, l, r) =>
        {
            (r.SetUpSideNode as Tpn.IValue).AssignTo((l.SetUpSideNode as Tpn.IMethod).Input()) ;
            return (l.SetUpSideNode as Tpn.IMethod).Returns();
        })
        {
        }
    }

    internal static class MemberUnwrapper{
        public static T Unwrap<T>(this IFrontendCodeElement codeElement) where T: IFrontendType
        {
            if (codeElement.Returns().Is< WeakMemberDefinition>(out var member) && 
                member.Type.IsDefinately(out var yes, out var _) &&  
                yes.Value.TypeDefinition.IsDefinately(out var yes2, out var _) &&
                yes2.Value.GetValue().IsDefinately(out var yes3, out var _) &&
                yes3.Value.Is<T>(out var t)) {
                return t;
            }
            return codeElement.Returns().Cast<T>();
        }
    }


}
