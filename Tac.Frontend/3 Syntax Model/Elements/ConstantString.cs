﻿using System.Linq;
using Tac.Frontend;
using Tac.Frontend._2_Parser;
using Tac.Model.Elements;
using Tac.Model.Instantiated;
using Tac.New;
using Tac.Parser;
using static Tac.Frontend.TransformerExtensions;

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
    internal class WeakConstantString : IFrontendCodeElement<IConstantString>
    {
        public WeakConstantString(IIsPossibly<string> value)
        {
            Value = value;
        }

        public IIsPossibly<string> Value { get; }

        public IBuildIntention<IConstantString> GetBuildIntention(ConversionContext context)
        {
            var (toBuild, maker) = ConstantString.Create();
            return new BuildIntention<IConstantString>(toBuild, () =>
            {
                maker.Build(
                    Value.GetOrThrow());
            });
        }

        public IIsPossibly<IFrontendType<IVerifiableType>> Returns()
        {
            return Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType());
        }
    }

    internal class ConstantStringMaker : IMaker<IPopulateScope<WeakConstantString>>
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


        public ITokenMatching<IPopulateScope<WeakConstantString>> TryMake(IMatchedTokenMatching tokenMatching)
        {
            var match = tokenMatching
                .Has(new StringMaker(), out var str);

            if (match
                 is IMatchedTokenMatching matched)
            {
                return TokenMatching<IPopulateScope<WeakConstantString>>.MakeMatch(matched.Tokens.Skip(1).ToArray(), matched.Context, new ConstantStringPopulateScope(str));
            }
            return TokenMatching<IPopulateScope<WeakConstantString>>.MakeNotMatch(tokenMatching.Context);
        }

        public static IPopulateScope<WeakConstantString> PopulateScope(string str)
        {
            return new ConstantStringPopulateScope(str);
        }
        public static IPopulateBoxes<WeakConstantString> PopulateBoxes(string str)
        {
            return new ConstantStringResolveReferance(str);
        }

        private class ConstantStringPopulateScope : IPopulateScope<WeakConstantString>
        {
            private readonly string str;

            public ConstantStringPopulateScope(string str)
            {
                this.str = str;
            }

            public IPopulateBoxes<WeakConstantString> Run(IPopulateScopeContext context)
            {
                return new ConstantStringResolveReferance(str);
            }

            public IBox<IIsPossibly<IFrontendType<IVerifiableType>>> GetReturnType()
            {
                return new Box<IIsPossibly<IFrontendType<IVerifiableType>>>(Possibly.Is(new _3_Syntax_Model.Elements.Atomic_Types.NumberType()));
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

            public IIsPossibly<WeakConstantString> Run(IResolveReferenceContext context)
            {
                return Possibly.Is(new WeakConstantString(Possibly.Is(str)));
            }
        }
    }



}
