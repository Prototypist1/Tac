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
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using Tac.SemanticModel;
using System.Linq;
using Prototypist.Toolbox.Object;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticIfSymbol = StaticSymbolsRegistry.AddOrThrow("then");
        public readonly string IfSymbol = StaticIfSymbol;
    }
}

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticIfMaker = AddOperationMatcher(() => new IfTrueOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> IfMaker = StaticIfMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}

namespace Tac.SemanticModel.Operations
{
    internal class WeakIfTrueOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IIfOperation>, IReturn
    {
        // right should have more validation
        public WeakIfTrueOperation(IOrType<IBox<IFrontendCodeElement>,IError> left, IOrType<IBox<IFrontendCodeElement>,IError> right) : base(left, right)
        {
        }


        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType());


        public override IBuildIntention<IIfOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = IfOperation.Create();
            return new BuildIntention<IIfOperation>(toBuild, () =>
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

            if (!Right.Possibly1().AsEnummerable().OfType<WeakBlockDefinition>().Any())
            {
                yield return Error.Other($"right hand side must be a block");
            }

            foreach (var error in Left.TypeCheck(new Tac.SyntaxModel.Elements.AtomicTypes.BlockType()))
            {
                yield return error;
            }
        }
    }

    internal class IfTrueOperationMaker : BinaryOperationMaker<WeakIfTrueOperation,IIfOperation>
    {
        public IfTrueOperationMaker() : base(SymbolsRegistry.StaticIfSymbol, (l,r)=> new Box<WeakIfTrueOperation>(new WeakIfTrueOperation(l,r)), (s, c, l, r) =>
        {
            l
            .TransformInner(x => x.SetUpSideNode)
            .TransformAndFlatten(x => x.SafeIs(out Tpn.ILookUpType lookup) ? OrType.Make<Tpn.ILookUpType, IError>(lookup) : throw new NotImplementedException("left should be a look up type, but I don't know where or how the error should happen"))
            .IfNotError(x => c.TypeProblem.IsNumber(s, x));

            return OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("bool"), new PlaceholderValueConverter()));
        })
        {
        }
    }
}
