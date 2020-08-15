using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.Infastructure;
using Tac.Parser;
using Prototypist.Toolbox.Object;
using Prototypist.Toolbox;
using Tac.SemanticModel.CodeStuff;
using Prototypist.Toolbox.Bool;

namespace Tac.Frontend.Parser
{
    // yep, it's a singleton
    // relax tho, it is really readonly
    //public class Keywords
    //{

    //    public static readonly Lazy<Keywords> Singleton = new Lazy<Keywords>(() => new Keywords());

    //    // TODO you have a sumbols registry!
    //    // 
    //    public readonly string Type;
    //    public readonly string Plus;
    //    public readonly string Minus;
    //    public readonly string Equal;
    //    public readonly string LessThan;
    //    public readonly string AssignNext;
    //    public readonly string AssignLast;
    //    public readonly string Then;
    //    public readonly string Else;
    //    public readonly string CallNext;
    //    public readonly string CallLast;
    //    public readonly string Path;
    //    public readonly string Return;
    //    public readonly string TypeOr;
    //    public readonly IReadOnlyList<string> AllKeywords;

    //    public Keywords()
    //    {
    //        var all = new List<string>();

    //        Type = Add("Type");
    //        Plus = Add("+");
    //        Minus = Add("-");
    //        Plus = Add("=");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        Plus = Add("+");
    //        TypeOr = Add("|");


    //        AllKeywords = all;

    //        string Add(string toAdd)
    //        {
    //            all.Add(toAdd);
    //            return toAdd;
    //        }
    //    }
    //}



    internal class NameMaker : IMaker<AtomicToken>
    {
        public ITokenMatching<AtomicToken> TryMake(IMatchedTokenMatching elementToken)
        {
            var index = elementToken.EndIndex;

            if (elementToken.AllTokens.Count > index &&
                elementToken.AllTokens[index] is AtomicToken first &&
                !double.TryParse(first.Item, out var _) &&
                IsNotKeyWord(first.Item))
            {
                return TokenMatching<AtomicToken>.MakeMatch(elementToken.AllTokens, elementToken.Context, first, index, index + 1);
            }

            return TokenMatching<AtomicToken>.MakeNotMatch(elementToken.Context);
        }

        private bool IsNotKeyWord(string item)
        {
            return StaticSymbolsRegistry.SymbolsRegistry.Symbols.Contains(item).Not();//.!Keywords.Singleton.Value.AllKeywords;
        }
    }

    internal class TypeNameMaker : IMaker<IKey>
    {
        public ITokenMatching<IKey> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index &&
                self.AllTokens[index] is AtomicToken first &&
                !double.TryParse(first.Item, out var _) &&
                IsNotKeyWord(first.Item))
            {

                var at = TokenMatching<NameKey>.MakeStart(self.AllTokens, self.Context, index);
                var match = new GenericNMaker().TryMake(at);
                if (match is IMatchedTokenMatching<IKey[]> mathced)
                {

                    return TokenMatching<IKey>.MakeMatch(
                        self.AllTokens,
                        self.Context,
                        new GenericNameKey(new NameKey(first.Item), mathced.Value.Select(x=>OrType.Make<IKey,IError>(x)).ToArray()),
                        index,
                        mathced.EndIndex);
                }

                return TokenMatching<IKey>.MakeMatch(self.AllTokens, self.Context, new NameKey(first.Item), index, index+1);
            }

            return TokenMatching<IKey>.MakeNotMatch(self.Context);
        }

        private bool IsNotKeyWord(string item)
        {
            return StaticSymbolsRegistry.SymbolsRegistry.Symbols.Contains(item).Not();
        }
    }

    internal class NumberMaker : IMaker<double>
    {
        public ITokenMatching<double> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index &&
                self.AllTokens[index] is AtomicToken first &&
                double.TryParse(first.Item, out var res))
            {
                return TokenMatching<double>.MakeMatch(self.AllTokens, self.Context, res, index, index +1);
            }

            return TokenMatching<double>.MakeNotMatch(self.Context);
        }
    }

    internal class DoneMaker : IMaker
    {
        public ITokenMatching TryMake(ITokenMatching self)
        {
            if (self is IMatchedTokenMatching matched && matched.AllTokens.Count >= matched.EndIndex )
            {
                return self;
            }

            // smells a little... <object>
            return TokenMatching<object>.MakeNotMatch(self.Context);
        }
    }

    internal class BodyMaker : IMaker<CurleyBracketToken>
    {
        public ITokenMatching<CurleyBracketToken> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index &&
                self.AllTokens[index] is CurleyBracketToken first)
            {
                return TokenMatching<CurleyBracketToken>.MakeMatch(self.AllTokens, self.Context, first, index, index + 1);
            }

            return TokenMatching<CurleyBracketToken>.MakeNotMatch(self.Context);
        }
    }

    internal class DefineGenericNMaker : IMaker<string[]>
    {
        public ITokenMatching<string[]> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index &&
                           self.AllTokens[index] is SquareBacketToken typeParameters &&
                               typeParameters.Tokens.All(x => x is LineToken line &&
                                   line.Tokens.Count == 1 &&
                                   line.Tokens.ElementAt(0) is AtomicToken))
            {
                return TokenMatching<string[]>.MakeMatch(
                    self.AllTokens, 
                    self.Context, 
                    typeParameters.Tokens.Select(x => x.CastTo<LineToken>().Tokens.Single().CastTo<AtomicToken>().Item).ToArray(), 
                    index,
                    index+1);
            }

            return TokenMatching<string[]>.MakeNotMatch(self.Context);
        }
    }

    internal class GenericNMaker : IMaker<IKey[]>
    {
        public ITokenMatching<IKey[]> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index &&
                self.AllTokens[index] is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is LineToken lt && lt.Tokens.All(y => y is AtomicToken)) &&
                TryToToken(out var res))
            {
                return TokenMatching<IKey[]>.MakeMatch(
                    self.AllTokens,
                    self.Context,
                    res!,
                    index,
                    index + 1);
            }

            return TokenMatching<IKey[]>.MakeNotMatch(self.Context);

            bool TryToToken(out IKey[]? typeSourcesInner)
            {
                var typeSourcesBuilding = new List<IKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<LineToken>())
                {
                    var matcher = TokenMatching<object>.MakeStart(elementToken.Tokens, self.Context,0);
                    IKey? typeSource = null;
                    if (matcher
                            .Has(new TypeNameMaker(), out typeSource)
                            .Has(new DoneMaker()) is IMatchedTokenMatching)
                    {
                        typeSourcesBuilding.Add(typeSource);
                    }
                    else
                    {
                        typeSourcesInner = default;
                        return false;
                    }
                }
                typeSourcesInner = typeSourcesBuilding.ToArray();
                return true;
            }
        }
    }

    internal class BinaryOperationMatcher : IMaker<(IToken lhs, AtomicToken token, IToken rhs)>
    {
        private readonly string s;

        public BinaryOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IToken, AtomicToken, IToken)> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count-1 > index & index != 0 &&
                    self.AllTokens[index] is AtomicToken op &&
                    op.Item == s)
            {
                return TokenMatching<(IToken, AtomicToken, IToken)>.MakeMatch(self.AllTokens, self.Context, (self.AllTokens[index-1], op, self.AllTokens[index+1]),
                    index,
                    index + 1);
            }
            

            return TokenMatching<(IToken, AtomicToken, IToken)>.MakeNotMatch(self.Context);
        }
    }

    
    internal class TrailingOperationMatcher : IMaker<(IToken perface, AtomicToken atom)>
    {
        private readonly string s;

        public TrailingOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IToken, AtomicToken)> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index & index != 1 &&
                    self.AllTokens[index] is AtomicToken op &&
                    op.Item == s)
            {
                return TokenMatching<(IToken, AtomicToken)>.MakeMatch(self.AllTokens, self.Context, (self.AllTokens[index - 1], op),
                    index,
                    index + 1);
            }


            return TokenMatching<(IToken, AtomicToken)>.MakeNotMatch(self.Context);
        }
    }

    internal class KeyWordMaker : IMaker<AtomicToken>
    {
        private readonly string word;

        public KeyWordMaker(string word)
        {
            this.word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public ITokenMatching<AtomicToken> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count > index && self.AllTokens[index] is AtomicToken first &&
                                first.Item == word)
            {
                return TokenMatching<AtomicToken>.MakeMatch(self.AllTokens, self.Context, first,
                    index,
                    index + 1);
            }
            return TokenMatching<AtomicToken>.MakeNotMatch(self.Context);
        }
    }
}
