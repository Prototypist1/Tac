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
using System.Linq;
using System.Collections.Generic;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticElseSymbol = StaticSymbolsRegistry.AddOrThrow("else");
        public readonly string ElseSymbol = StaticElseSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticElseMaker = AddOperationMatcher(() => new ElseOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ElseMaker = StaticElseMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    // really an if not
    internal class WeakElseOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IElseOperation>, IReturn
    {
        // right should have more validation
        public WeakElseOperation(IOrType<IBox<IFrontendCodeElement>, IError> left, IOrType<IBox<IFrontendCodeElement>, IError> right) : base(left, right)
        {
        }
        
        public override IBuildIntention<IElseOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ElseOperation.Create();
            return new BuildIntention<IElseOperation>(toBuild, () =>
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

            if (leftList.Any())
            {
                var left = leftList.First();
                if (!left.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType()))
                {
                    yield return Error.Other($"left cannot be {left}");
                }
            }

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
                .OfType<IIsDefinately<IFrontendType>>()
                .Select(x => x.Value.UnwrapRefrence())
                .ToArray();

            if (rightList.Any()) { 
                var right = leftList.First();
                if (!right.IsAssignableTo(new Tac.SyntaxModel.Elements.AtomicTypes.BlockType())) {
                    yield return Error.Other($"right cannot be {right}");

                }
            }
        }
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation,IElseOperation>
    {
        public ElseOperationMaker() : base(SymbolsRegistry.StaticElseSymbol, (l,r)=>new Box<WeakElseOperation>(new WeakElseOperation(l,r)), (s, c, l, r) => OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"),new PlaceholderValueConverter())))
        {
        }
    }
    
}
