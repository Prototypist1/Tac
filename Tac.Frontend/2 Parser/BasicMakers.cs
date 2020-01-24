using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.New;
using Tac.Parser;
using Prototypist.Toolbox.Object;

namespace Tac.Frontend.Parser
{
    // yep, it's a singleton
    // relax tho, it is really readonly
    public class Keywords
    {

        public static readonly Lazy<Keywords> Singleton = new Lazy<Keywords>(() => new Keywords());

        public readonly string Type;
        public readonly IReadOnlyList<string> AllKeywords;

        public Keywords()
        {
            var all = new List<string>();

            Type = Add("Type");


            AllKeywords = all;

            string Add(string toAdd)
            {
                all.Add(toAdd);
                return toAdd;
            }
        }
    }

    internal class NameMaker : IMaker<AtomicToken>
    {
        public ITokenMatching<AtomicToken> TryMake(IMatchedTokenMatching elementToken)
        {
            if (elementToken.Tokens.Any() &&
                elementToken.Tokens[0] is AtomicToken first &&
                !double.TryParse(first.Item, out var _) &&
                IsNotKeyWord(first.Item))
            {
                return TokenMatching<AtomicToken>.MakeMatch(elementToken.Tokens.Skip(1).ToArray(), elementToken.Context, first);
            }

            return TokenMatching<AtomicToken>.MakeNotMatch(elementToken.Context);
        }

        private bool IsNotKeyWord(string item)
        {
            return !Keywords.Singleton.Value.AllKeywords.Contains(item);
        }
    }

    internal class TypeNameMaker : IMaker<IKey>
    {
        public ITokenMatching<IKey> TryMake(IMatchedTokenMatching self)
        {

            if (self.Tokens.Any() &&
                self.Tokens[0] is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching<NameKey>.MakeStart(self.Tokens.Skip(1).ToArray(), self.Context);
                var match = new GenericNMaker().TryMake(at);
                if (match is IMatchedTokenMatching<IKey[]> mathced)
                {
                    return TokenMatching<IKey>.MakeMatch(
                        self.Tokens.Skip(2).ToArray(),
                        self.Context,
                        new GenericNameKey(new NameKey(first.Item), mathced.Value));
                }

                return TokenMatching<IKey>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, new NameKey(first.Item));
            }

            return TokenMatching<IKey>.MakeNotMatch(self.Context);
        }
    }

    internal class NumberMaker : IMaker<double>
    {
        public ITokenMatching<double> TryMake(IMatchedTokenMatching self)
        {
            if (self.Tokens.Any() &&
                self.Tokens[0] is AtomicToken first &&
                double.TryParse(first.Item, out var res))
            {
                return TokenMatching<double>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, res);
            }

            return TokenMatching<double>.MakeNotMatch(self.Context);
        }
    }

    internal class DoneMaker : IMaker
    {
        public ITokenMatching TryMake(ITokenMatching self)
        {
            if (self is IMatchedTokenMatching matched && !matched.Tokens.Any())
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

            if (self.Tokens.Any() &&
                self.Tokens[0] is CurleyBracketToken first)
            {
                return TokenMatching<CurleyBracketToken>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, first);
            }

            return TokenMatching<CurleyBracketToken>.MakeNotMatch(self.Context);
        }
    }

    internal class DefineGenericNMaker : IMaker<string[]>
    {
        public ITokenMatching<string[]> TryMake(IMatchedTokenMatching elementMatching)
        {

            if (elementMatching.Tokens.Any() &&
                           elementMatching.Tokens[0] is SquareBacketToken typeParameters &&
                               typeParameters.Tokens.All(x => x is LineToken line &&
                                   line.Tokens.Count == 1 &&
                                   line.Tokens.ElementAt(0) is ElementToken element &&
                                   element.Tokens.Count == 1 &&
                                   element.Tokens.ElementAt(0) is AtomicToken))
            {
                return TokenMatching<string[]>.MakeMatch(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context, typeParameters.Tokens.Select(x => x.CastTo<LineToken>().Tokens.Single().CastTo<ElementToken>().Tokens.Single().CastTo<AtomicToken>().Item).ToArray());
            }

            return TokenMatching<string[]>.MakeNotMatch(elementMatching.Context);
        }
    }

    internal class GenericNMaker : IMaker<IKey[]>
    {
        public ITokenMatching<IKey[]> TryMake(IMatchedTokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens[0] is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is LineToken lt && lt.Tokens.All(y => y is ElementToken)) &&
                TryToToken(out var res))
            {
                return TokenMatching<IKey[]>.MakeMatch(
                    elementMatching.Tokens.Skip(1).ToArray(),
                    elementMatching.Context,
                    res!);
            }

            return TokenMatching<IKey[]>.MakeNotMatch(elementMatching.Context);

            bool TryToToken(out IKey[]? typeSourcesInner)
            {
                var typeSourcesBuilding = new List<IKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<LineToken>())
                {
                    var matcher = TokenMatching<object>.MakeStart(elementToken.Tokens, elementMatching.Context);
                    IKey? typeSource = null;
                    if (matcher
                        .HasElement(x => x
                            .Has(new TypeNameMaker(), out typeSource)
                            .Has(new DoneMaker()))
                        .Has(new DoneMaker()) is IMatchedTokenMatching)
                    {
                        typeSourcesBuilding.Add(typeSource!);
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

    internal class BinaryOperationMatcher : IMaker<(IReadOnlyList<IToken>, AtomicToken, IToken)>
    {
        private readonly string s;

        public BinaryOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IReadOnlyList<IToken>, AtomicToken, IToken)> TryMake(IMatchedTokenMatching elementMatching)
        {
            var right = elementMatching.Tokens[elementMatching.Tokens.Count - 1];
            if (elementMatching.Tokens.Any() &&
                            (right is ParenthesisToken ||
                            right is ElementToken)
                            )
            {

                var at = TokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)>.MakeStart(elementMatching.Tokens.Take(elementMatching.Tokens.Count - 1).ToArray(), elementMatching.Context);

                if (at.Tokens.Any() &&
                    at.Tokens[at.Tokens.Count - 1] is AtomicToken op &&
                    op.Item == s)
                {
                    var preface = at.Tokens.Take(at.Tokens.Count - 1).ToArray();
                    return TokenMatching<(IReadOnlyList<IToken>, AtomicToken, IToken)>.MakeMatch(preface, elementMatching.Context, (at.Tokens.Take(at.Tokens.Count - 1).ToArray(), op, right));
                }
            }

            return TokenMatching<(IReadOnlyList<IToken>, AtomicToken, IToken)>.MakeNotMatch(elementMatching.Context);
        }
    }

    internal class TrailingOperationMatcher : IMaker<(IEnumerable<IToken>, AtomicToken)>
    {
        private readonly string s;

        public TrailingOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IEnumerable<IToken>, AtomicToken)> TryMake(IMatchedTokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens[elementMatching.Tokens.Count - 1] is AtomicToken op
                && op.Item == s)
            {

                var preface = elementMatching.Tokens.Take(elementMatching.Tokens.Count - 1).ToArray(); ;
                return TokenMatching<(IEnumerable<IToken>, AtomicToken)>.MakeMatch(preface, elementMatching.Context, (preface, op));
            }

            return TokenMatching<(IEnumerable<IToken>, AtomicToken)>.MakeNotMatch(elementMatching.Context);
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
            if (self.Tokens[0] is AtomicToken first &&
                                first.Item == word)
            {
                return TokenMatching<AtomicToken>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, first);
            }
            return TokenMatching<AtomicToken>.MakeNotMatch(self.Context);
        }
    }

    internal class AndDoneMaker<T> : IMaker<T>
    {
        private readonly IMaker<T> backing;

        public AndDoneMaker(IMaker<T> backing)
        {
            this.backing = backing ?? throw new ArgumentNullException(nameof(backing));
        }

        public ITokenMatching<T> TryMake(IMatchedTokenMatching elementToken)
        {
            var first = backing.TryMake(elementToken);

            if (!(first is IMatchedTokenMatching<T> firstMatching))
            {
                return first;
            }

            var second = first.Has(new DoneMaker());

            if (second is IMatchedTokenMatching secondMatching)
            {
                return TokenMatching<T>.MakeMatch(secondMatching.Tokens, second.Context, firstMatching.Value);
            }

            return TokenMatching<T>.MakeNotMatch(second.Context);
        }
    }
}
