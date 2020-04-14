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
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel.CodeStuff;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;

namespace Tac.SemanticModel.CodeStuff
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
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticLastCallMaker = AddOperationMatcher(() => new LastCallOperationMaker());

        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticNextCallMaker = AddOperationMatcher(() => new NextCallOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> LastCallMaker = StaticLastCallMaker;
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> NextCallMaker = StaticNextCallMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    internal class WeakNextCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, INextCallOperation>
    {
        public WeakNextCallOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<INextCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = NextCallOperation.Create();
            return new BuildIntention<INextCallOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)), 
                    Right.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal class NextCallOperationMaker : BinaryOperationMaker<WeakNextCallOperation,INextCallOperation>
    {
        public NextCallOperationMaker() : base(SymbolsRegistry.StaticNextCallSymbol, (l,r)=> new Box<WeakNextCallOperation>( new WeakNextCallOperation(l,r)),(s,c,l,r)=> {

            // nearly duplicate code 3930174039475
            if (l.Is2(out var error1)){
                return OrType.Make<Tpn.IValue, IError>(error1);
            }

            if (r.Is2(out var error2)) {
                return OrType.Make<Tpn.IValue, IError>(error2);
            }

            var left = l.Is1OrThrow();
            var right = r.Is1OrThrow();

            if (!(left is Tpn.ICanAssignFromMe assignFrom)) {
                return OrType.Make<Tpn.IValue, IError>(new Error("can not assign from the left"));
            }

            if (!(right is Tpn.IValue value))
            {
                return OrType.Make<Tpn.IValue, IError>(new Error("right is not value"));
            }

            s.Problem.IsAssignedTo(assignFrom, value.Input());
            return OrType.Make<Tpn.IValue, IError>(value.Returns()); 
        })
        {
        }
    }

    internal class WeakLastCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILastCallOperation>
    {
        public const string Identifier = "<";

        public WeakLastCallOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ILastCallOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = LastCallOperation.Create();
            return new BuildIntention<ILastCallOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)), 
                    Right.TransformInner(x => x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal class LastCallOperationMaker : BinaryOperationMaker<WeakLastCallOperation, ILastCallOperation>
    {
        public LastCallOperationMaker() : base(SymbolsRegistry.StaticLastCallSymbol, (l,r)=>new Box<WeakLastCallOperation>( new WeakLastCallOperation(l,r)), (s, c, l, r) =>
        {
            // nearly duplicate code 3930174039475
            if (l.Is2(out var error1))
            {
                return OrType.Make<Tpn.IValue, IError>(error1);
            }

            if (r.Is2(out var error2))
            {
                return OrType.Make<Tpn.IValue, IError>(error2);
            }

            var left = l.Is1OrThrow();
            var right = r.Is2OrThrow();

            if (!(left is Tpn.IValue value))
            {
                return OrType.Make<Tpn.IValue, IError>(new Error("left is not value"));
            }

            if (!(right is Tpn.ICanAssignFromMe assignFrom ))
            {
                return OrType.Make<Tpn.IValue, IError>(new Error("can not assign from the right" ));
            }

            s.Problem.IsAssignedTo(assignFrom, value.Input());
            return OrType.Make<Tpn.IValue, IError>(value.Returns());
        })
        {
        }
    }
}
