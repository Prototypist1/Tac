using Prototypist.LeftToRight;
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Model;
using Tac.New;
using Tac.Parser;

namespace Tac.Frontend._2_Parser
{
    // yep, it's a singleton
    // relax tho, it is really readonly
    public class Keywords {

        public static Lazy<Keywords> Singleton = new Lazy<Keywords>(()=>new Keywords());

        public readonly string Type;
        public readonly IReadOnlyList<string> AllKeywords;

        public Keywords()
        {
            var all = new List<string>();

            Type = Add("Type");


            AllKeywords = all;

            string Add(string toAdd) {
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
                elementToken.Tokens.First() is AtomicToken first &&
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

    internal class TypeNameMaker : IMaker<NameKey>
    {
        public ITokenMatching<NameKey> TryMake(IMatchedTokenMatching self)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching<NameKey>.MakeStart(self.Tokens.Skip(1).ToArray(), self.Context);
                var match = new GenericNMaker().TryMake(at);
                if (match is IMatchedTokenMatching<NameKey[]> mathced)
                {
                    return TokenMatching<NameKey>.MakeMatch(
                        self.Tokens.Skip(2).ToArray(), 
                        self.Context, 
                        new GenericNameKey(new NameKey(first.Item), mathced.Value));
                }
                
                return TokenMatching<NameKey>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, new NameKey(first.Item));
            }
            
            return TokenMatching<NameKey>.MakeNotMatch(self.Context);
        }
    }

    internal class NumberMaker : IMaker<double>
    {
        public ITokenMatching<double> TryMake(IMatchedTokenMatching self)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item, out var res))
            {
                return TokenMatching<double>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context,res);
            }
            
            return TokenMatching<double>.MakeNotMatch(self.Context);
        }
    }

    internal class DoneMaker : IMaker
    {
        public ITokenMatching TryMake(ITokenMatching self)
        {
            if (self is IMatchedTokenMatching matched && !matched.Tokens.Any()) {
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
                self.Tokens.First() is CurleyBracketToken first)
            {
                return TokenMatching< CurleyBracketToken>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, first);
            }
            
            return TokenMatching<CurleyBracketToken>.MakeNotMatch(self.Context);
        }
    }

    internal class Generic3Maker : IMaker<(AtomicToken, AtomicToken, AtomicToken)>
    {
        public ITokenMatching<(AtomicToken, AtomicToken, AtomicToken)> TryMake(IMatchedTokenMatching self)
        {

            if (self.Tokens.Any() &&
                            self.Tokens.First() is SquareBacketToken typeParameters &&
                                typeParameters.Tokens.Count() == 3 &&
                                typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                                    firstLine.Tokens.Count() == 1 &&
                                    firstLine.Tokens.ElementAt(0) is ElementToken firstElement &&
                                    firstElement.Tokens.Count() == 1 &&
                                    firstElement.Tokens.ElementAt(0) is AtomicToken firstType &&
                                typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                                    secondLine.Tokens.Count() == 1 &&
                                    secondLine.Tokens.ElementAt(0) is ElementToken secondElement &&
                                    secondElement.Tokens.Count() == 1 &&
                                    secondElement.Tokens.ElementAt(0) is AtomicToken secondType &&
                                typeParameters.Tokens.ElementAt(2) is LineToken thridLine &&
                                    thridLine.Tokens.Count() == 1 &&
                                    thridLine.Tokens.ElementAt(0) is ElementToken thridElement &&
                                    thridElement.Tokens.Count() == 1 &&
                                    thridElement.Tokens.ElementAt(0) is AtomicToken thirdType)
            {
                return TokenMatching<(AtomicToken, AtomicToken, AtomicToken)>.MakeMatch(self.Tokens.Skip(1).ToArray(), self.Context, (firstType,secondType,thirdType));
            }
            
            return TokenMatching<(AtomicToken, AtomicToken, AtomicToken)>.MakeNotMatch(self.Context);

        }
    }

    internal class DefineGenericNMaker : IMaker<string[]>
    {
        public ITokenMatching<string[]> TryMake(IMatchedTokenMatching elementMatching)
        {
            
            if (elementMatching.Tokens.Any() &&
                           elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                               typeParameters.Tokens.All(x => x is LineToken line &&
                                   line.Tokens.Count() == 1 &&
                                   line.Tokens.ElementAt(0) is ElementToken element &&
                                   element.Tokens.Count() == 1 &&
                                   element.Tokens.ElementAt(0) is AtomicToken))
            {
                return TokenMatching<string[]>.MakeMatch(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context, typeParameters.Tokens.Select(x => x.Cast<LineToken>().Tokens.Single().Cast<ElementToken>().Tokens.Single().Cast<AtomicToken>().Item).ToArray());
            }
            
            return TokenMatching<string[]>.MakeNotMatch(elementMatching.Context);
        }
    }

    internal class GenericNMaker : IMaker<NameKey[]>
    {
        public ITokenMatching<NameKey[]> TryMake(IMatchedTokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is LineToken lt && lt.Tokens.All(y=> y is ElementToken)) &&
                TryToToken(out var res))
            {
                return TokenMatching<NameKey[]>.MakeMatch(
                    elementMatching.Tokens.Skip(1).ToArray(), 
                    elementMatching.Context,
                    res);
            }
            
            return TokenMatching<NameKey[]>.MakeNotMatch(elementMatching.Context);

            bool TryToToken(out NameKey[] typeSourcesInner)
            {
                var typeSourcesBuilding = new List<NameKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<LineToken>())
                {
                    var matcher = TokenMatching<object>.MakeStart(elementToken.Tokens, elementMatching.Context);
                    NameKey typeSource = null;
                    if (matcher
                        .HasElement(x=>x
                        .Has(new TypeNameMaker(), out typeSource)
                        .Has(new DoneMaker()))
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

    internal class Generic2Maker : IMaker<(AtomicToken, AtomicToken)>
    {
        public ITokenMatching<(AtomicToken, AtomicToken)> TryMake(IMatchedTokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                    typeParameters.Tokens.Count() == 2 &&
                    typeParameters.Tokens.ElementAt(0) is LineToken firstLine &&
                        firstLine.Tokens.Count() == 1 &&
                        firstLine.Tokens.ElementAt(0) is ElementToken firstElement &&
                        firstElement.Tokens.Count() == 1 &&
                        firstElement.Tokens.ElementAt(0) is AtomicToken firstType &&
                    typeParameters.Tokens.ElementAt(1) is LineToken secondLine &&
                        secondLine.Tokens.Count() == 1 &&
                        secondLine.Tokens.ElementAt(0) is ElementToken secondElement &&
                        secondElement.Tokens.Count() == 1 &&
                        secondElement.Tokens.ElementAt(0) is AtomicToken secondType)
            {
                return TokenMatching<(AtomicToken, AtomicToken)>.MakeMatch(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context, (firstType, secondType));
            }
            
            return TokenMatching<(AtomicToken, AtomicToken)>.MakeNotMatch( elementMatching.Context);

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
            if (elementMatching.Tokens.Any() &&
                            (elementMatching.Tokens.Last() is ParenthesisToken ||
                            elementMatching.Tokens.Last() is ElementToken)
                            )
            {
                var right = elementMatching.Tokens.Last();

                var at = TokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)>.MakeStart(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(), elementMatching.Context);

                if (at.Tokens.Any() &&
                    at.Tokens.Last() is AtomicToken op &&
                    op.Item == s)
                {
                    var preface = at.Tokens.Take(at.Tokens.Count() - 1).ToArray();
                    return TokenMatching<(IReadOnlyList<IToken>, AtomicToken, IToken)>.MakeMatch(preface, elementMatching.Context, (at.Tokens.Take(at.Tokens.Count() - 1).ToArray(), op, right));
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
                elementMatching.Tokens.Last() is AtomicToken op)
            {

                var preface = elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(); ;
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
            if (self.Tokens.First() is AtomicToken first &&
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

            if (!(first is IMatchedTokenMatching<T> firstMatching)) {
                return first;
            }

            var second = first.Has(new DoneMaker());

            if (second is IMatchedTokenMatching secondMatching) {
                return TokenMatching<T>.MakeMatch(secondMatching.Tokens, second.Context, firstMatching.Value);
            }

            return TokenMatching<T>.MakeNotMatch(second.Context);
        }
    }

    //internal class MatchOneMaker<T> : IMaker<T> {
    //    private readonly IMaker<T> left, right;

    //    public MatchOneMaker(IMaker<T> left, IMaker<T> right)
    //    {
    //        this.left = left ?? throw new ArgumentNullException(nameof(left));
    //        this.right = right ?? throw new ArgumentNullException(nameof(right));
    //    }

    //    public ITokenMatching<T> TryMake(ITokenMatching elementToken)
    //    {
    //        var leftRes = left.TryMake(elementToken);
    //        var rightRes = right.TryMake(elementToken);

    //        if (leftRes.HasValue && rightRes.HasValue) {
    //            throw new Exception("both should not match");
    //        }

    //        if (leftRes.HasValue) {
    //            return leftRes;
    //        }

    //        if (rightRes.HasValue) {
    //            return rightRes;
    //        }

    //        // the choice of left or right is meaningless 
    //        return leftRes;
    //    }
    //}

    //internal class MatchOneMaker : IMaker
    //{
    //    private readonly IMaker left, right;

    //    public MatchOneMaker(IMaker left, IMaker right)
    //    {
    //        this.left = left ?? throw new ArgumentNullException(nameof(left));
    //        this.right = right ?? throw new ArgumentNullException(nameof(right));
    //    }

    //    public ITokenMatching TryMake(ITokenMatching elementToken)
    //    {
    //        var leftRes = left.TryMake(elementToken);
    //        var rightRes = right.TryMake(elementToken);

    //        if (!leftRes.IsNotMatch && !rightRes.IsNotMatch)
    //        {
    //            throw new Exception("both should not match");
    //        }

    //        if (!leftRes.IsNotMatch)
    //        {
    //            return leftRes;
    //        }

    //        if (!rightRes.IsNotMatch)
    //        {
    //            return rightRes;
    //        }

    //        // the choice of left or right is meaningless 
    //        return leftRes;
    //    }
    //}
}
