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
using Tac.SyntaxModel.Elements.AtomicTypes;

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

    internal static class CallOperationSharedCode {
        public static IEnumerable<IError> Validate(IOrType<IBox<IFrontendCodeElement>, IError> input, IOrType<IBox<IFrontendCodeElement>, IError> method) {
            var inputTypeOrErrors = input.ReturnsTypeOrErrors();
            var methodInputTypeOrErrors = method
                .ReturnsTypeOrErrors()
                .TransformAndFlatten(thing => thing.TryGetInput().SwitchReturns(orType=> orType, no=> OrType.Make<IFrontendType, IError>(Error.Other($"{thing} should return")), error => OrType.Make<IFrontendType, IError>(error)));

            return inputTypeOrErrors.SwitchReturns(
                i => methodInputTypeOrErrors.SwitchReturns<IEnumerable<IError>>(
                    mi=> {
                        if (!i.TheyAreUs(mi).SwitchReturns(x => x, x => false))
                        {
                            return new[] { Error.Other($"{method} does not accept {input}") };
                        }
                        return Array.Empty<IError>();
                    },
                    mi=> {
                        return new IError[] { mi };
                    }), 
                i => methodInputTypeOrErrors.SwitchReturns<IEnumerable<IError>>(
                    m => {
                        return new IError[] { i };
                    }, 
                    m => {
                        return new IError[] { i,m };
                    }));
        }

        public static IOrType<IFrontendType, IError> Returns(IOrType<IBox<IFrontendCodeElement>, IError> method) { 
            return method.ReturnsTypeOrErrors().TransformAndFlatten<IFrontendType, IFrontendType>(thing => {
                if (thing is Tac.SyntaxModel.Elements.AtomicTypes.MethodType method)
                {
                    return method.OutputType;
                }

                return OrType.Make<IFrontendType, IError>(Error.Other($"{thing} should return"));
            });
        }
    }

    internal class WeakNextCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, INextCallOperation>, IReturn
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
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IOrType<IFrontendType, IError> Returns()=>CallOperationSharedCode.Returns(Right);
        

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            foreach (var error in CallOperationSharedCode.Validate(Left, Right)) {
                yield return error;
            }

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
                return OrType.Make<Tpn.IValue, IError>(Error.Other("can not assign from the left"));
            }

            if (!(right is Tpn.IValue value))
            {
                return OrType.Make<Tpn.IValue, IError>(Error.Other("right is not value"));
            }

            s.Problem.IsAssignedTo(assignFrom, value.Input());
            return OrType.Make<Tpn.IValue, IError>(value.Returns()); 
        })
        {
        }
    }

    internal class WeakLastCallOperation : BinaryOperation<IFrontendCodeElement, IFrontendCodeElement, ILastCallOperation>, IReturn
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
                    Left.Is1OrThrow().GetValue().ConvertElementOrThrow(context), 
                    Right.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }


        public IOrType<IFrontendType, IError> Returns() => CallOperationSharedCode.Returns(Left);

        public override IEnumerable<IError> Validate()
        {
            foreach (var error in base.Validate())
            {
                yield return error;
            }

            foreach (var error in CallOperationSharedCode.Validate(Right, Left))
            {
                yield return error;
            }

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
                return OrType.Make<Tpn.IValue, IError>(Error.Other("left is not value"));
            }

            if (!(right is Tpn.ICanAssignFromMe assignFrom ))
            {
                return OrType.Make<Tpn.IValue, IError>(Error.Other("can not assign from the right" ));
            }

            s.Problem.IsAssignedTo(assignFrom, value.Input());
            return OrType.Make<Tpn.IValue, IError>(value.Returns());
        })
        {
        }
    }
}
