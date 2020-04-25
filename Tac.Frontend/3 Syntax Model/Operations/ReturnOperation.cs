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
using Tac.Frontend.Parser;
using Prototypist.Toolbox.Object;
using Prototypist.Toolbox;
using Tac.SemanticModel;
using System.Linq;

namespace Tac.SemanticModel.CodeStuff
{
    // this is how we register the symbol
    public partial class SymbolsRegistry
    {
        public static readonly string StaticReturnSymbol = StaticSymbolsRegistry.AddOrThrow("return");
        public readonly string ReturnSymbol = StaticReturnSymbol;
    }
}


namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticReturnMaker = AddOperationMatcher(() => new ReturnOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ReturnMaker = StaticReturnMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{
    internal class WeakReturnOperation : TrailingOperation, IConvertableFrontendCodeElement<IReturnOperation>
    {
        public WeakReturnOperation(IOrType< IBox<IFrontendCodeElement>,IError> result)
        {
            Result = result;
        }
        
        public IOrType<IBox<IFrontendCodeElement>, IError> Result { get; }
        
        public IBuildIntention<IReturnOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ReturnOperation.Create();
            return new BuildIntention<IReturnOperation>(toBuild, () =>
            {
                maker.Build(Result.Is1OrThrow().GetValue().ConvertElementOrThrow(context));
            });
        }

        public IEnumerable<IError> Validate()
        {
            foreach (var error in Result.SwitchReturns(x=>x.GetValue().Validate(),x=>new[] { x }))
            {
                yield return error;
            }
        }

        public IEnumerable<IError> Validate()
        {
            // TODO this goes in the base class!
            foreach (var error in Result.SwitchReturns(x => x.GetValue().Validate(), x => new List<IError>() { x }))
            {
                yield return error;
            }

            var intermittentResult = Result.Possibly1().AsEnummerable()
                .Select(x => x.GetValue()).ToArray();

            foreach (var thing in intermittentResult)
            {
                if (!(thing is IReturn))
                {
                    yield return Error.Other($"{thing} should return");
                }
            }
        }
    }

    internal abstract class TrailingOperion<T> 
    {
        public abstract IConvertableFrontendCodeElement<ICodeElement>[] Operands { get; }
        public abstract T1 Convert<T1,TBacking>(IOpenBoxesContext<T1,TBacking> context) where TBacking:IBacking;
        public abstract IVerifiableType Returns();
    }

    internal class TrailingOperation {
        public delegate OrType<Tpn.IValue,IError> GetReturnedValue(Tpn.IScope scope, ISetUpContext context, IOrType<ISetUpResult<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>,IError> parm);

        public delegate IBox<T> Make<out T>(IOrType< IBox<IFrontendCodeElement>,IError> codeElement);


    }

    internal class TrailingOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>>
        where TFrontendCodeElement : class, IConvertableFrontendCodeElement<TCodeElement>
        where TCodeElement : class, ICodeElement
    {
        private readonly TrailingOperation.GetReturnedValue getReturnedValue;

        public TrailingOperationMaker(string symbol, TrailingOperation.Make<TFrontendCodeElement> make, TrailingOperation.GetReturnedValue getReturnedValue)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Make = make ?? throw new ArgumentNullException(nameof(make));
            this.getReturnedValue = getReturnedValue ?? throw new ArgumentNullException(nameof(getReturnedValue));
        }

        public string Symbol { get; }
        private TrailingOperation.Make<TFrontendCodeElement> Make { get; }

        public ITokenMatching<ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var matching = tokenMatching
                .HasStruct(new TrailingOperationMatcher(Symbol), out var _);

            return matching.ConvertIfMatched(res => new TrailingPopulateScope(matching.Context.ParseLine(res.perface), Make, getReturnedValue));
        }
        
        private class TrailingPopulateScope : ISetUp<IBox<TFrontendCodeElement>, Tpn.IValue>
        {
            private readonly IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;
            private readonly TrailingOperation.GetReturnedValue getReturnedValue;

            public TrailingPopulateScope(IOrType<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>, IError> left, TrailingOperation.Make<TFrontendCodeElement> make, TrailingOperation.GetReturnedValue getReturnedValue)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.getReturnedValue = getReturnedValue ?? throw new ArgumentNullException(nameof(getReturnedValue));
            }

            public ISetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.TransformInner(x=>x.Run(scope, context));
                return new SetUpResult<IBox<TFrontendCodeElement>, Tpn.IValue>(
                    new TrailingResolveReferance(nextLeft.TransformInner(x=>x.Resolve), make), 
                    getReturnedValue(scope, context, nextLeft));
            }
        }

        private class TrailingResolveReferance: IResolve<IBox<TFrontendCodeElement>>
        {
            public readonly IOrType<IResolve<IBox<IFrontendCodeElement>>, IError> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;

            public TrailingResolveReferance(IOrType<IResolve<IBox<IFrontendCodeElement>>,IError> resolveReferance1, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IBox<TFrontendCodeElement> Run(Tpn.ITypeSolution context)
            {
                return make(left.TransformInner(x => x.Run(context)));
            }
        }
    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation, IReturnOperation>
    {
        public ReturnOperationMaker() : base(SymbolsRegistry.StaticReturnSymbol, x=>new Box<WeakReturnOperation>(new WeakReturnOperation(x)),(s,c,x)=> {


            // this smells
            // I am using this delegate for more than it was ment to do

            var  mem = c.TypeProblem.GetReturns(s);

            if (x.Is2(out var error)) {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            var val1 = x.Is1OrThrow();

            if (val1.SetUpSideNode.Is2(out var error2))
            {
                return OrType.Make<Tpn.IValue, IError>(error);
            }

            var val1node= val1.SetUpSideNode.Is1OrThrow();

            if (!(val1node is Tpn.IValue finalVal))
            {
                return OrType.Make<Tpn.IValue, IError>(Error.Other($"can not return {val1node}"));
            }


            // this is shit, with good code this is fine, but with bad code this will throw
            // I will need to change this when I do a pass to communitcate error better
            finalVal.AssignTo(mem);

            return OrType.Make<Tpn.IValue, IError>( c.TypeProblem.CreateValue(s, new NameKey("empty"), new PlaceholderValueConverter()));
        })
        {
        }
    }
}
