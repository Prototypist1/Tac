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

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticConstantBoolMaker = AddElementMakers(
            () => new ConstantBoolMaker(),
            MustBeBefore<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable CA1823
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<ISetUp<IFrontendCodeElement, Tpn.ITypeProblemNode>> ConstantBoolMaker = StaticConstantBoolMaker;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823
    }
}


namespace Tac.SemanticModel.Operations
{

    internal class WeakConstantBool : IConvertableFrontendCodeElement<IConstantBool>
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
    }

    internal class ConstantBoolMaker : IMaker<ISetUp<WeakConstantBool, Tpn.IValue>>
    {
        public ConstantBoolMaker() { }


        internal class BoolMaker : IMaker<bool>
        {
            public ITokenMatching<bool> TryMake(IMatchedTokenMatching self)
            {
                if (self.Tokens.Any() &&
                    self.Tokens[0] is AtomicToken first)
                {
                    if (first.Item == "true") {
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, true);

                    }
                    else if (first.Item == "false"){
                        return TokenMatching<bool>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, false);

                    }

                }

                return TokenMatching<bool>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<ISetUp<WeakConstantBool, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .HasStruct(new BoolMaker(), out var dub);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<ISetUp<WeakConstantBool, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantBoolPopulateScope(dub));
            }
            return TokenMatching<ISetUp<WeakConstantBool, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static ISetUp<WeakConstantBool, Tpn.IValue> PopulateScope(bool dub)
        {
            return new ConstantBoolPopulateScope(dub);
        }
        public static IResolve<WeakConstantBool> PopulateBoxes(bool dub)
        {
            return new ConstantBoolResolveReferance(dub);
        }

        private class ConstantBoolPopulateScope : ISetUp<WeakConstantBool, Tpn.IValue>
        {
            private readonly bool dub;

            public ConstantBoolPopulateScope(bool dub)
            {
                this.dub = dub;
            }

            public ISetUpResult<WeakConstantBool, Tpn.IValue> Run(Tpn.IScope scope, ISetUpContext context)
            {
                // PlaceholderValueConverter is a little weird
                // I kind of think it should make the WeakConstantBool
                // yeah it totally should
                // TODO 
                // this applies to my other constants 
                var value = context.TypeProblem.CreateValue(scope, new NameKey("bool"), new PlaceholderValueConverter());
                return new SetUpResult<WeakConstantBool, Tpn.IValue>(new ConstantBoolResolveReferance(dub),new OrType<Tpn.IValue,IError>(value));
            }
        }

        private class ConstantBoolResolveReferance : IResolve<WeakConstantBool>
        {
            private readonly bool dub;

            public ConstantBoolResolveReferance(
                bool dub)
            {
                this.dub = dub;
            }

            public IBox<WeakConstantBool> Run(Tpn.ITypeSolution context)
            {
                return new Box<WeakConstantBool>(new WeakConstantBool(Possibly.Is(dub)));
            }
        }
    }
}
