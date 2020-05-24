using System.Linq;
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

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> StaticConstantStringMaker = AddElementMakers(
            () => new ConstantStringMaker(),
            MustBeBefore<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IBox<IFrontendCodeElement>, Tpn.ITypeProblemNode>> ConstantStringMaker = StaticConstantStringMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    // TODO how does this work???
    // is it returnable?
    // no
    // it returns a number?
    // one might say all numbers are the same
    // but we do know more about constants
    // I guess maybe there should be a class number extended by constant number?
    // IDK!
    internal class WeakConstantString : IConvertableFrontendCodeElement<IConstantString>, IReturn
    {
        public WeakConstantString(IIsPossibly<string> value)
        {
            Value = value;
        }

        public IIsPossibly<string> Value { get; }

        public IBuildIntention<IConstantString> GetBuildIntention(IConversionContext context)
        {
            var (toBuild, maker) = ConstantString.Create();
            return new BuildIntention<IConstantString>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IOrType<IFrontendType, IError> Returns() => OrType.Make<IFrontendType, IError>(new Tac.SyntaxModel.Elements.AtomicTypes.StringType());

        public IEnumerable<IError> Validate() => Array.Empty<IError>();
    }

    internal class ConstantStringMaker : IMaker<ISetUp<IBox<WeakConstantString>, Tpn.IValue>>
    {
        public ConstantStringMaker() { }


        private class StringMaker : IMaker<string>
        {
            public ITokenMatching<string> TryMake(IMatchedTokenMatching self)
            {
                if (self.Tokens.Any() &&
                    self.Tokens[0] is AtomicToken first &&
                    first.Item.StartsWith('"') && first.Item.EndsWith('"'))
                {
                    var res = first.Item[1..^1];
                    return TokenMatching<string>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, res);
                }

                return TokenMatching<string>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<ISetUp<IBox<WeakConstantString>, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new StringMaker(), out var str);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<IBox<WeakConstantString>, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantStringPopulateScope(str!));
            }
            return TokenMatching<ISetUp<IBox<WeakConstantString>, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<IBox<WeakConstantString>, Tpn.IValue> PopulateScope(string str)
        {
            return new ConstantStringPopulateScope(str);
        }
        public static IResolve<IBox<WeakConstantString>> PopulateBoxes(string str)
        {
            return new ConstantStringResolveReferance(str);
        }

        private class ConstantStringPopulateScope : ISetUp<IBox<WeakConstantString>, Tpn.IValue>
        {
            private readonly string str;

            public ConstantStringPopulateScope(string str)
            {
                this.str = str;
            }

            public ISetUpResult<IBox<WeakConstantString>, Tpn.IValue> Run(Tpn.IStaticScope scope, ISetUpContext context)
            {
                var value = context.TypeProblem.CreateValue(scope, new NameKey("string"), new PlaceholderValueConverter());
                return new SetUpResult<IBox<WeakConstantString>, Tpn.IValue>(new ConstantStringResolveReferance(str),OrType.Make<Tpn.IValue,IError>(value));
            }
        }

        private class ConstantStringResolveReferance : IResolve<IBox<WeakConstantString>>
        {
            private readonly string str;

            public ConstantStringResolveReferance(
                string str)
            {
                this.str = str;
            }

            public IBox<WeakConstantString> Run(Tpn.ITypeSolution context)
            {
                return new Box<WeakConstantString>(new WeakConstantString(Possibly.Is(str)));
            }
        }
    }



}
