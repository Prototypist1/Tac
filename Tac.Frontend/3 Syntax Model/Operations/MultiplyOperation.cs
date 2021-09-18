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
using Prototypist.Toolbox.Object;

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

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns() => OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType());

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            foreach (var error in Left.TypeCheck(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType()))
            {
                yield return error;
            }

            foreach (var error in Right.TypeCheck(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType()))
            {
                yield return error;
            }
        }

    }

    internal class MultiplyOperationMaker : BinaryOperationMaker<WeakMultiplyOperation, IMultiplyOperation>
    {
        public MultiplyOperationMaker() : base(SymbolsRegistry.StaticMultiplySymbols, (l,r)=>new Box<WeakMultiplyOperation>(new WeakMultiplyOperation(l,r)), (s, c, l, r) => {

            if (!(s is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            l
            .TransformInner(x => x.SetUpSideNode)
            .TransformAndFlatten(x => x.SafeIs(out Tpn.ILookUpType lookup) ? OrType.Make<Tpn.ILookUpType, IError>(lookup) : throw new NotImplementedException("left should be a look up type, but I don't know where or how the error should happen"))
            .IfNotError(x => c.TypeProblem.IsNumber(runtimeScope, x));

            r
            .TransformInner(x => x.SetUpSideNode)
            .TransformAndFlatten(x => x.SafeIs(out Tpn.ILookUpType lookup) ? OrType.Make<Tpn.ILookUpType, IError>(lookup) : throw new NotImplementedException("right should be a look up type, but I don't know where or how the error should happen"))
            .IfNotError(x => c.TypeProblem.IsNumber(runtimeScope, x));


            return OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(runtimeScope, new NameKey("number")));
        }, true)
        {
        }
    }
}
