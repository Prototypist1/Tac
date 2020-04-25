using System;
using System.Collections.Generic;
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
using Prototypist.Toolbox;
using Tac.SemanticModel;
using System.Linq;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox< IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticLessThanMaker = AddOperationMatcher(() => new LessThanOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> LessThanMaker = StaticLessThanMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticLessThanSymbol = StaticSymbolsRegistry.AddOrThrow("<?");
        public readonly string LessThanSymbol = StaticLessThanSymbol;
    }
    

    internal class WeakLessThanOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILessThanOperation>, IReturn
    {
        public WeakLessThanOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<ILessThanOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = LessThanOperation.Create();
            return new BuildIntention<ILessThanOperation>(toBuild, () =>
            {
                maker.Build(
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType());

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
                .OfType<IIsDefinately<IFrontendType>>()
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

            if (leftList.Length == rightList.Length)
            {
                foreach (var error in leftList.Zip(rightList, (leftReturns, rightReturns) => {
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

    internal class LessThanOperationMaker : BinaryOperationMaker<WeakLessThanOperation, ILessThanOperation>
    {
        public LessThanOperationMaker() : base(SymbolsRegistry.StaticLessThanSymbol, (l,r)=> new Box<WeakLessThanOperation>(new WeakLessThanOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"), new PlaceholderValueConverter())))
        {
        }
    }
}
