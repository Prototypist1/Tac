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
using Prototypist.Toolbox.Bool;
using Prototypist.Toolbox.Object;

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

            if (!Right.Possibly1().AsEnummerable().OfType<WeakBlockDefinition>().Any()) {
                yield return Error.Other($"right hand side must be a block");
            }

            foreach (var error in Left.TypeCheck(new Tac.SyntaxModel.Elements.AtomicTypes.BlockType()))
            {
                yield return error;
            }
        }
    }


    internal class ElseOperationMaker : BinaryOperationMaker<WeakElseOperation,IElseOperation>
    {
        public ElseOperationMaker() : base(SymbolsRegistry.StaticElseSymbol, (l,r)=>new Box<WeakElseOperation>(new WeakElseOperation(l,r)), (s, c, l, r) => {

            if (!(s is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            l
            .TransformInner(x => x.SetUpSideNode)
            .TransformAndFlatten(x => x.SafeIs(out Tpn.ILookUpType lookup) ? OrType.Make<Tpn.ILookUpType, IError>(lookup) : throw new NotImplementedException("left should be a look up type, but I don't know where or how the error should happen"))
            .IfNotError(x => c.TypeProblem.IsNumber(runtimeScope, x));

            return OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(runtimeScope, new NameKey("bool"), new PlaceholderValueConverter())); })
        {
        }
    }
    
}
