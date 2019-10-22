using System.Linq;
using Tac._3_Syntax_Model.Elements.Atomic_Types;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Frontend.New;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model;
using Tac.Semantic_Model.Operations;

namespace Tac.Parser
{

    internal partial class MakerRegistry
    {
        private static readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> StaticConstantStringMaker = AddElementMakers(
            () => new ConstantStringMaker(),
            MustBeBefore<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>>(typeof(MemberMaker)));
#pragma warning disable IDE0052 // Remove unread private members
        private readonly WithConditions<IPopulateScope<IFrontendCodeElement, Tpn.ITypeProblemNode>> ConstantStringMaker = StaticConstantStringMaker;
#pragma warning restore IDE0052 // Remove unread private members
    }
}


namespace Tac.Semantic_Model.Operations
{

    // TODO how does this work???
    // is it returnable?
    // no
    // it returns a number?
    // one might say all numbers are the same
    // but we do know more about constants
    // I guess maybe there should be a class number extended by constant number?
    // IDK!
    internal class WeakConstantString : IConvertableFrontendCodeElement<IConstantString>
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

        public IIsPossibly<IFrontendType> Returns()
        {
            return Possibly.Is<IFrontendType>(PrimitiveTypes.CreateNumberType());
        }
    }

    internal class ConstantStringMaker : IMaker<IPopulateScope<WeakConstantString, Tpn.IValue>>
    {
        public ConstantStringMaker() { }


        private class StringMaker : IMaker<string>
        {
            public ITokenMatching<string> TryMake(IMatchedTokenMatching self)
            {
                if (self.Tokens.Any() &&
                    self.Tokens.First() is AtomicToken first &&
                    first.Item.StartsWith('"') && first.Item.EndsWith('"'))
                {
                    var res = first.Item.Substring(1, first.Item.Length - 2);
                    return TokenMatching<string>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, res);
                }

                return TokenMatching<string>.MakeNotMatch(self.Context);
            }
        }


        public ITokenMatching<IPopulateScope<WeakConstantString, Tpn.IValue>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new StringMaker(), out var str);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantString, Tpn.IValue>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantStringPopulateScope(str));
            }
            return TokenMatching<IPopulateScope<WeakConstantString, Tpn.IValue>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakConstantString, Tpn.IValue> PopulateScope(string str)
        {
            return new ConstantStringPopulateScope(str);
        }
        public static IPopulateBoxes<WeakConstantString> PopulateBoxes(string str)
        {
            return new ConstantStringResolveReferance(str);
        }

        private class ConstantStringPopulateScope : IPopulateScope<WeakConstantString, Tpn.IValue>
        {
            private readonly string str;

            public ConstantStringPopulateScope(string str)
            {
                this.str = str;
            }

            public IResolvelizeScope<WeakConstantString, Tpn.IValue> Run(Tpn.IScope scope, IPopulateScopeContext context)
            {
                var value = context.TypeProblem.CreateValue(scope, new NameKey("string"));
                return new ConstantStringFinalizeScope(str, value);
            }
        }

        private class ConstantStringFinalizeScope : IResolvelizeScope<WeakConstantString, Tpn.IValue>
        {
            private readonly string str;

            public ConstantStringFinalizeScope(string str, Tpn.IValue setUpSideNode)
            {
                this.str = str;
                SetUpSideNode = setUpSideNode ?? throw new System.ArgumentNullException(nameof(setUpSideNode));
            }

            public Tpn.IValue SetUpSideNode  {get;}


            public IPopulateBoxes<WeakConstantString> Run(IResolvableScope parent, IFinalizeScopeContext context)
            {
                return new ConstantStringResolveReferance(str);
            }
        }

        private class ConstantStringResolveReferance : IPopulateBoxes<WeakConstantString>
        {
            private readonly string str;

            public ConstantStringResolveReferance(
                string str)
            {
                this.str = str;
            }

            public IIsPossibly<WeakConstantString> Run(IResolvableScope scope, IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakConstantString(Possibly.Is(str)));
            }
        }
    }



}
