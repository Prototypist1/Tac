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
using Prototypist.Toolbox.Object;

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


    internal class WeakAddOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, IAddOperation>, IReturn
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

        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.NumberType());

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

    internal class AddOperationMaker : BinaryOperationMaker<WeakAddOperation, IAddOperation>
    {
        public AddOperationMaker() : base(SymbolsRegistry.StaticAddSymbol, (l,r)=> new Box<WeakAddOperation>(new WeakAddOperation(l, r)),(s,c,l,r)=> {
            IError error;

            if (l.Is2(out error))
            {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            if (r.Is2(out error))
            {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            var left = l.Is1OrThrow().SetUpSideNode;
            var right = r.Is1OrThrow().SetUpSideNode;

            if (left.Is2(out error))
            {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            if (right.Is2(out error))
            {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            if (!left.Is1OrThrow().SafeIs(out Tpn.ILookUpType leftLookup))
            {
                return OrType.Make<Tpn.IValue, IError>(Error.Other("left should be a look up"));
            }

            c.TypeProblem.IsNumber(s,leftLookup);

            if (!right.Is1OrThrow().SafeIs(out Tpn.ILookUpType rightLookup))
            {
                return OrType.Make<Tpn.IValue, IError>(Error.Other("right should be a look up"));
            }

            c.TypeProblem.IsNumber(s,rightLookup);

            return OrType.Make<Tpn.IValue, IError>(c.TypeProblem.CreateValue(s, new NameKey("number"), new PlaceholderValueConverter()));
        }){}
    }

}
