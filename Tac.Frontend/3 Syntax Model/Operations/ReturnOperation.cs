﻿using System;
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
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticReturnMaker = AddOperationMatcher(() => new ReturnOperationMaker());
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ReturnMaker = StaticReturnMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{
    internal class WeakReturnOperation : TrailingOperation, IConvertableFrontendCodeElement<IReturnOperation>
    {
        public WeakReturnOperation(OrType< IBox<IFrontendCodeElement>,IError> result)
        {
            Result = result;
        }
        
        public OrType<IBox<IFrontendCodeElement>, IError> Result { get; }
        
        public IBuildIntention<IReturnOperation> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ReturnOperation.Create();
            return new BuildIntention<IReturnOperation>(toBuild, () =>
            {
                maker.Build(Result.Convert(x=>x.GetValue().ConvertElementOrThrow(context)));
            });
        }
    }

    internal abstract class TrailingOperion<T> 
    {
        public abstract IConvertableFrontendCodeElement<ICodeElement>[] Operands { get; }
        public abstract T1 Convert<T1,TBacking>(IOpenBoxesContext<T1,TBacking> context) where TBacking:IBacking;
        public abstract IVerifiableType Returns();
    }

    internal class TrailingOperation {
        public delegate Tpn.IValue GetReturnedValue(Tpn.IScope scope, ISetUpContext context, OrType<ISetUpResult<IFrontendCodeElement, Tpn.ITypeProblemNode>,IError> parm);

        public delegate IBox<T> Make<out T>(OrType< IBox<IFrontendCodeElement>,IError> codeElement);
    }

    internal class TrailingOperationMaker<TFrontendCodeElement, TCodeElement> : IMaker<ISetUp<TFrontendCodeElement, Tpn.IValue>>
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

        public ITokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            

            var matching = tokenMatching
                .HasStruct(new TrailingOperationMatcher(Symbol), out (IEnumerable<IToken> perface, AtomicToken _) res);
            if (matching is IMatchedTokenMatching matched)
            {
                var left = matching.Context.ParseLine(res.perface);
                
                return TokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>>.MakeMatch(
                    matched.Tokens,
                    matched.Context, 
                    new TrailingPopulateScope(left,Make, getReturnedValue));
            }
            return TokenMatching<ISetUp<TFrontendCodeElement, Tpn.IValue>>.MakeNotMatch(
                    matching.Context);
        }
        
        //public static ISetUp<TFrontendCodeElement, Tpn.IValue> PopulateScope(ISetUp<IConvertableFrontendCodeElement<ICodeElement>, Tpn.IValue> left,
        //        TrailingOperation.Make<TFrontendCodeElement> make, TrailingOperation.GetReturnedValue getReturnedValue)
        //{
        //    return new TrailingPopulateScope(left, make, getReturnedValue);
        //}

        //public static IResolve<TFrontendCodeElement> PopulateBoxes(IResolve<IConvertableFrontendCodeElement<ICodeElement>> left,
        //        TrailingOperation.Make<TFrontendCodeElement> make)
        //{
        //    return new TrailingResolveReferance(left,
        //        make);
        //}


        private class TrailingPopulateScope : ISetUp<TFrontendCodeElement, Tpn.IValue>
        {
            private readonly OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;
            private readonly TrailingOperation.GetReturnedValue getReturnedValue;

            public TrailingPopulateScope(OrType<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>, IError> left, TrailingOperation.Make<TFrontendCodeElement> make, TrailingOperation.GetReturnedValue getReturnedValue)
            {
                this.left = left ?? throw new ArgumentNullException(nameof(left));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
                this.getReturnedValue = getReturnedValue ?? throw new ArgumentNullException(nameof(getReturnedValue));
            }

            public ISetUpResult<TFrontendCodeElement, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                var nextLeft = left.Convert(x=>x.Run(scope, context));
                return new SetUpResult<TFrontendCodeElement, Tpn.IValue>(
                    new TrailingResolveReferance(nextLeft.Convert(x=>x.Resolve), make), 
                    getReturnedValue(scope, context, nextLeft));
            }
        }


        private class TrailingResolveReferance: IResolve<TFrontendCodeElement>
        {
            public readonly OrType<IResolve<IFrontendCodeElement>, IError> left;
            private readonly TrailingOperation.Make<TFrontendCodeElement> make;

            public TrailingResolveReferance(OrType<IResolve<IFrontendCodeElement>,IError> resolveReferance1, TrailingOperation.Make<TFrontendCodeElement> make)
            {
                left = resolveReferance1 ?? throw new ArgumentNullException(nameof(resolveReferance1));
                this.make = make ?? throw new ArgumentNullException(nameof(make));
            }

            public IBox<TFrontendCodeElement> Run(Tpn.ITypeSolution context)
            {
                return make(left.Convert(x => x.Run(context)));
            }
        }
    }


    internal class ReturnOperationMaker : TrailingOperationMaker<WeakReturnOperation, IReturnOperation>
    {
        public ReturnOperationMaker() : base(SymbolsRegistry.StaticReturnSymbol, x=>new Box<WeakReturnOperation>(new WeakReturnOperation(x)),(s,c,x)=> {


            // this smells
            // I am using this delegate for more than it was ment to do

            var  mem = c.TypeProblem.GetReturns(s);
            
            // this is shit, with good code this is fine, but with bad code this will throw
            // I will need to change this when I do a pass to communitcate error better
            x.SetUpSideNode.CastTo<Tpn.IValue>().AssignTo(mem);

            return c.TypeProblem.CreateValue(s, new NameKey("empty"), new PlaceholderValueConverter());
        })
        {
        }
    }
}
