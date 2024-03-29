﻿using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.Operations;
using Tac.Frontend.Parser;
using Prototypist.Toolbox;
using System;
using System.Collections.Generic;
using Tac.SemanticModel.CodeStuff;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticConstantNumberMaker = AddElementMakers(
            () => new ConstantNumberMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ConstantNumberMaker = StaticConstantNumberMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823

    }
}



namespace Tac.SemanticModel.Operations
{

    internal class WeakConstantNumber : IConvertableFrontendCodeElement<IConstantNumber>, IReturn
    {
        public WeakConstantNumber(IIsPossibly<double> value) 
        {
            Value = value;
        }

        public IIsPossibly<double> Value { get; }

        public IBuildIntention<IConstantNumber> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ConstantNumber.Create();
            return new BuildIntention<IConstantNumber>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns() => OrType.Make<IFrontendType<IVerifiableType>, IError>(new SyntaxModel.Elements.AtomicTypes.NumberType());

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal class ConstantNumberMaker : IMaker<ISetUp<IBox<WeakConstantNumber>, Tpn.IValue>>
    {
        public ConstantNumberMaker() {}

        public ITokenMatching<ISetUp<IBox<WeakConstantNumber>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new NumberMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<IBox<WeakConstantNumber>, Tpn.IValue>>.MakeMatch(tokenMatching, new ConstantNumberPopulateScope(dub), matched.EndIndex);
            }
            return TokenMatching<ISetUp<IBox<WeakConstantNumber>, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

    }

    internal class ConstantNumberPopulateScope : ISetUp<IBox<WeakConstantNumber>, Tpn.IValue>
    {
        private readonly double dub;

        public ConstantNumberPopulateScope(double dub)
        {
            this.dub = dub;
        }

        public ISetUpResult<IBox<WeakConstantNumber>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {
            scope = scope.EnterInitizaionScopeIfNessisary();

            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            var value = context.TypeProblem.CreateValue(runtimeScope, new NameKey("number"));
            return new SetUpResult<IBox<WeakConstantNumber>, Tpn.IValue>(new ConstantNumberResolveReferance(dub), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class ConstantNumberResolveReferance : IResolve<IBox<WeakConstantNumber>>
    {
        private readonly double dub;

        public ConstantNumberResolveReferance(
            double dub)
        {
            this.dub = dub;
        }

        public IBox<WeakConstantNumber> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            return new Box<WeakConstantNumber>(new WeakConstantNumber(Possibly.Is(dub)));
        }
    }
}
