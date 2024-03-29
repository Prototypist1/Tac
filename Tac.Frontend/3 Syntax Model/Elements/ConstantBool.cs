﻿using System.Linq;
using Tac.SyntaxModel.Elements.AtomicTypes;
using Tac.Frontend;
using Tac.Frontend.Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.Infastructure;
using Tac.Parser;
using Tac.SemanticModel;
using Tac.SemanticModel.Operations;
using Prototypist.Toolbox;
using System.Collections.Generic;
using System;
using Tac.SemanticModel.CodeStuff;
using Prototypist.Toolbox.Object;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticConstantBoolMaker = AddElementMakers(
            () => new ConstantBoolMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ConstantBoolMaker = StaticConstantBoolMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    internal class WeakConstantBool : IConvertableFrontendCodeElement<IConstantBool>, IReturn
    {
        public WeakConstantBool(IIsPossibly<bool> value)
        {
            Value = value;
        }

        public IIsPossibly<bool> Value { get; }

        public IBuildIntention<IConstantBool> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ConstantBool.Create();
            return new BuildIntention<IConstantBool>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IOrType<IFrontendType<IVerifiableType>, IError> Returns() => OrType.Make<IFrontendType<IVerifiableType>, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.BooleanType());

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal class ConstantBoolMaker : IMaker<ISetUp<IBox<WeakConstantBool>, Tpn.IValue>>
    {
        public ConstantBoolMaker() { }


        internal class BoolMaker : IMaker<bool>
        {
            public ITokenMatching<bool> TryMake(IMatchedTokenMatching self)
            {
                if (self.AllTokens.Count > self.EndIndex && 
                    self.AllTokens[self.EndIndex].Is1(out var token) && token.SafeIs(out AtomicToken first))
                {
                    if (first.Item == "true") {
                        return TokenMatching<bool>.MakeMatch(self, true, self.EndIndex +1);

                    }
                    else if (first.Item == "false"){
                        return TokenMatching<bool>.MakeMatch(self, false, self.EndIndex + 1);

                    }

                }

                return TokenMatching<bool>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<ISetUp<IBox<WeakConstantBool>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new BoolMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<IBox<WeakConstantBool>, Tpn.IValue>>.MakeMatch(tokenMatching, new ConstantBoolPopulateScope(dub), matched.EndIndex);
            }
            return TokenMatching<ISetUp<IBox<WeakConstantBool>, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        //public static ISetUp<IBox<WeakConstantBool>, Tpn.IValue> PopulateScope(bool dub)
        //{
        //    return new ConstantBoolPopulateScope(dub);
        //}
        //public static IResolve<IBox<WeakConstantBool>> PopulateBoxes(bool dub)
        //{
        //    return new ConstantBoolResolveReferance(dub);
        //}

    }


    internal class ConstantBoolPopulateScope : ISetUp<IBox<WeakConstantBool>, Tpn.IValue>
    {
        private readonly bool dub;

        public ConstantBoolPopulateScope(bool dub)
        {
            this.dub = dub;
        }

        public ISetUpResult<IBox<WeakConstantBool>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
        {

            scope = scope.EnterInitizaionScopeIfNessisary();

            if (!(scope is Tpn.IScope runtimeScope))
            {
                throw new NotImplementedException("this should be an IError");
            }

            // PlaceholderValueConverter is a little weird
            // I kind of think it should make the WeakConstantBool
            // yeah it totally should
            // TODO 
            // this applies to my other constants 
            var value = context.TypeProblem.CreateValue(runtimeScope, new NameKey("bool"));
            return new SetUpResult<IBox<WeakConstantBool>, Tpn.IValue>(new ConstantBoolResolveReferance(dub), OrType.Make<Tpn.IValue, IError>(value));
        }
    }

    internal class ConstantBoolResolveReferance : IResolve<IBox<WeakConstantBool>>
    {
        private readonly bool dub;

        public ConstantBoolResolveReferance(
            bool dub)
        {
            this.dub = dub;
        }

        public IBox<WeakConstantBool> Run(Tpn.TypeSolution context, IEnumerable<Tpn.ITypeProblemNode> stack)
        {
            return new Box<WeakConstantBool>(new WeakConstantBool(Possibly.Is(dub)));
        }
    }
}
