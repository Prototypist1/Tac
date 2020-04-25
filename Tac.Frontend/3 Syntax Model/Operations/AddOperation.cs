using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.SyntaxModel.Elements.AtomicTypes;
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
        public static readonly string StaticAddSymbol = StaticSymbolsRegistry.AddOrThrow("+");
        public readonly string AddSymbol = StaticAddSymbol;
    }

}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticAddMaker = AddOperationMatcher(()=> new AddOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> AddMaker = StaticAddMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}

namespace Tac.SemanticModel.Operations
{


    internal class WeakAddOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAddOperation>
    {
        public WeakAddOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }

        public override IBuildIntention<IAddOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = AddOperation.Create();
            return new BuildIntention<IAddOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context),
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            var intermittentLeft = Left.Possibly1().AsEnummerable()
                .Select(x => x.GetValue()).ToArray();

            foreach (var thing in intermittentLeft) {
                if (!(thing is IReturn)) {
                    yield return Error.Other($"{thing} should return");
                }
            }

            var leftList = intermittentLeft
                .OfType<IReturn>()
                .ToArray();

            var intermittentRight = Right.Possibly1().AsEnummerable()
                .Select(x => x.GetValue()).ToArray();


            foreach (var thing in intermittentRight)
            {
                if (!(thing is IReturn))
                {
                    yield return Error.Other($"{thing} should return");
                }
            }

            var rightList = intermittentRight
                .OfType<IReturn>()
                .ToArray();

            if (leftList.Length == rightList.Length)
            {
                foreach (var error in leftList.Zip(rightList, (x, y) => {
                    var leftReturns = x.Returns();
                    var rightReturns = y.Returns();
                    if (leftReturns.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType()) && rightReturns.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType()))
                    {
                        return Possibly.IsNot<IError>();
                    }
                    else
                    {
                        return Possibly.Is(Error.Other($"cannot add {leftReturns} to {rightReturns}"));
                    }
                }).OfType<IIsDefinately<IError>>().Select(x => x.Value))
                {
                    yield return error;
                }
            }
        }
    }

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation, IAddOperation>
    {
        public AddOperationMaker() : base(SymbolsRegistry.StaticAddSymbol, (l,r)=> new Box<WeakAddOperation>(new WeakAddOperation(l, r)),(s,c,l,r)=> OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("number"),new PlaceholderValueConverter())))
        {
        }
    }

}
