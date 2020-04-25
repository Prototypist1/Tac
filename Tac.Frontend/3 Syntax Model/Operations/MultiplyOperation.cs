using System;
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
using System.Collections.Generic;
using System.Linq;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticMultiplySymbols = StaticSymbolsRegistry.AddOrThrow("*");
        public readonly string MultiplySymbols = StaticMultiplySymbols;
    }
}


namespace Tac.Parser
{
    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticMultiplyMaker = AddOperationMatcher(() => new MultiplyOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> MultiplyMaker = StaticMultiplyMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{
    internal class WeakMultiplyOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IMultiplyOperation>, IReturn
    {
        public const string Identifier = "*";

        public WeakMultiplyOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IMultiplyOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = MultiplyOperation.Create();
            return new BuildIntention<IMultiplyOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType());

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            var intermittentLeft = Left.Possibly1().AsEnummerable()
                .Select(x => x.GetValue()).ToArray();

            foreach (var thing in intermittentLeft)
            {
                if (!(thing is IReturn))
                {
                    yield return Error.Other($"{thing} should return");
                }
            }

            var leftList = intermittentLeft
                .OfType<IReturn>()
                .Select(x => x.Returns().Possibly1())
                .OfType<IIsDefinately<IFrontendType>>() // I really need a safe OfType
                .Select(x => x.Value.UnwrapRefrence())
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
                .Select(x => x.Returns().Possibly1())
                .OfType<IIsDefinately<IFrontendType>>() // I really need a safe OfType
                .Select(x => x.Value.UnwrapRefrence())
                .ToArray();

            if (rightList.Any() && leftList.Any())
            {
                var left = rightList.First();
                var right = leftList.First();
                if (!(left.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType()) && right.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType())))
                {
                    yield return Error.Other($"can not multiply {left} with {right}");
                }
            }
        }

    }

    internal class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation, IMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(SymbolsRegistry.StaticMultiplySymbols, (l,r)=>new Box<WeakMultiplyOperation>(new WeakMultiplyOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("number"),new PlaceholderValueConverter())))
        {
        }
    }
}
