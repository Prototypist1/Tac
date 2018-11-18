using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tac.New;
using Tac.Parser;
using Tac.Semantic_Model.Names;

namespace Tac.Frontend._2_Parser
{
    internal class NameMaker : IMaker<AtomicToken>
    {
        public ITokenMatching<AtomicToken> TryMake(ITokenMatching elementToken)
        {
            if (elementToken.Tokens.Any() &&
                elementToken.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                return TokenMatching<AtomicToken>.Match(elementToken.Tokens.Skip(1).ToArray(), elementToken.Context, first);
            }
            
            return TokenMatching<AtomicToken>.NotMatch(elementToken.Tokens, elementToken.Context);
        }
    }

    internal class TypeMaker : IMaker<NameKey>
    {
        public ITokenMatching<NameKey> TryMake(ITokenMatching self)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                !double.TryParse(first.Item, out var _))
            {
                var at = TokenMatching<NameKey>.Start(self.Tokens.Skip(1), self.Context);
                var match = new GenericNMaker().TryMake(at);
                if (match.HasValue)
                {
                    return TokenMatching<NameKey>.Match(self.Tokens.Skip(2).ToArray(), self.Context, new GenericNameKey(new NameKey(first.Item), match.Value));
                }
                
                return TokenMatching<NameKey>.Match(self.Tokens.Skip(1).ToArray(), self.Context, new NameKey(first.Item));
            }
            
            return TokenMatching<NameKey>.NotMatch(self.Tokens, self.Context);
        }
    }

    internal class NumberMaker : IMaker<double>
    {
        public ITokenMatching<double> TryMake(ITokenMatching self)
        {
            if (self.Tokens.Any() &&
                self.Tokens.First() is AtomicToken first &&
                double.TryParse(first.Item, out var res))
            {
                return TokenMatching<double>.Match(self.Tokens.Skip(1).ToArray(), self.Context,res);
            }
            
            return TokenMatching<double>.NotMatch(self.Tokens, self.Context);
        }
    }

    internal class DoneMaker : IMaker
    {
        public ITokenMatching TryMake(ITokenMatching self)
        {
            if (!self.Tokens.Any())
            {
                return self;
            }

            // smells a little...
            return TokenMatching<object>.NotMatch(self.Tokens, self.Context);
        }
    }


    internal class BodyMaker : IMaker<CurleyBracketToken>
    {
        public ITokenMatching<CurleyBracketToken> TryMake(ITokenMatching self)
        {

            if (self.Tokens.Any() &&
                self.Tokens.First() is CurleyBracketToken first)
            {
                return TokenMatching< CurleyBracketToken>.Match(self.Tokens.Skip(1).ToArray(), self.Context, first);
            }
            
            return TokenMatching<CurleyBracketToken>.NotMatch(self.Tokens, self.Context);
        }
    }

    internal class Generic3Maker : IMaker<(AtomicToken, AtomicToken, AtomicToken)>
    {
        public ITokenMatching<(AtomicToken, AtomicToken, AtomicToken)> TryMake(ITokenMatching self)
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
                return TokenMatching<(AtomicToken, AtomicToken, AtomicToken)>.Match(self.Tokens.Skip(1).ToArray(), self.Context, (firstType,secondType,thirdType));
            }
            
            return TokenMatching<(AtomicToken, AtomicToken, AtomicToken)>.NotMatch(self.Tokens, self.Context);

        }
    }

    internal class DefineGenericNMaker : IMaker<AtomicToken[]>
    {
        public ITokenMatching<AtomicToken[]> TryMake(ITokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                           elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                               typeParameters.Tokens.All(x => x is LineToken firstLine &&
                                   firstLine.Tokens.Count() == 1 &&
                                   firstLine.Tokens.ElementAt(0) is AtomicToken))
            {
                return TokenMatching<AtomicToken[]>.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context, typeParameters.Tokens.Select(x => (x as LineToken).Tokens.First() as AtomicToken).ToArray());
            }
            
            return TokenMatching<AtomicToken[]>.NotMatch(elementMatching.Tokens, elementMatching.Context);

        }
    }

    internal class GenericNMaker : IMaker<NameKey[]>
    {
        public ITokenMatching<NameKey[]> TryMake(ITokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.First() is SquareBacketToken typeParameters &&
                typeParameters.Tokens.All(x => x is ElementToken) &&
                TryToToken(out var res))
            {
                return TokenMatching<NameKey[]>.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context,res);
            }
            
            return TokenMatching<NameKey[]>.NotMatch(elementMatching.Tokens, elementMatching.Context);

            bool TryToToken(out NameKey[] typeSourcesInner)
            {
                var typeSourcesBuilding = new List<NameKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<ElementToken>())
                {
                    var matcher = TokenMatching<object>.Start(elementToken.Tokens, elementMatching.Context);
                    if (matcher.Has(new TypeMaker(), out NameKey typeSource).Has(new DoneMaker()).IsNotMatch)
                    {
                        typeSourcesInner = default;
                        return false;
                    }
                    else
                    {
                        typeSourcesBuilding.Add(typeSource);
                    }
                }
                typeSourcesInner = typeSourcesBuilding.ToArray();
                return true;
            }
        }
    }

    internal class Generic2Maker : IMaker<(AtomicToken, AtomicToken)>
    {
        public ITokenMatching<(AtomicToken, AtomicToken)> TryMake(ITokenMatching elementMatching)
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
                return TokenMatching<(AtomicToken, AtomicToken)>.Match(elementMatching.Tokens.Skip(1).ToArray(), elementMatching.Context, (firstType, secondType));
            }
            
            return TokenMatching<(AtomicToken, AtomicToken)>.NotMatch(elementMatching.Tokens, elementMatching.Context);

        }
    }

    internal class BinaryOperationMaker : IMaker<(IEnumerable<IToken>, AtomicToken, IToken)>
    {
        private readonly string s;

        public BinaryOperationMaker(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)> TryMake(ITokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                            (elementMatching.Tokens.Last() is ParenthesisToken ||
                            elementMatching.Tokens.Last() is ElementToken)
                            )
            {
                var right = elementMatching.Tokens.Last();

                var at = TokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)>.Start(elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(), elementMatching.Context);

                if (at.Tokens.Any() &&
                    at.Tokens.Last() is AtomicToken op &&
                    op.Item == s)
                {
                    var preface = at.Tokens.Take(at.Tokens.Count() - 1);
                    return TokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)>.Match(preface, elementMatching.Context, (at.Tokens.Take(at.Tokens.Count() - 1), op, right));
                }
            }
            
            return TokenMatching<(IEnumerable<IToken>, AtomicToken, IToken)>.NotMatch(elementMatching.Tokens, elementMatching.Context);
        }
    }
    
    internal class TrailingOperationMaker : IMaker<(IEnumerable<IToken>, AtomicToken)>
    {
        private readonly string s;

        public TrailingOperationMaker(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(IEnumerable<IToken>, AtomicToken)> TryMake(ITokenMatching elementMatching)
        {
            if (elementMatching.Tokens.Any() &&
                elementMatching.Tokens.Last() is AtomicToken op)
            {

                var preface = elementMatching.Tokens.Take(elementMatching.Tokens.Count() - 1).ToArray(); ;
                return TokenMatching<(IEnumerable<IToken>, AtomicToken)>.Match(preface, elementMatching.Context, (preface, op));
            }
            
            return TokenMatching<(IEnumerable<IToken>, AtomicToken)>.NotMatch(elementMatching.Tokens, elementMatching.Context);
        }
    }

    internal class KeyWordMaker : IMaker<AtomicToken>
    {
        private readonly string word;

        public KeyWordMaker(string word)
        {
            this.word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public ITokenMatching<AtomicToken> TryMake(ITokenMatching self)
        {
            if (self.Tokens.First() is AtomicToken first &&
                                first.Item == word)
            {
                return TokenMatching<AtomicToken>.Match(self.Tokens.Skip(1).ToArray(), self.Context, first);
            }
            return TokenMatching<AtomicToken>.NotMatch(self.Tokens, self.Context);
        }
    }

    internal class MatchOneMaker<T> : IMaker<T> {
        private readonly IMaker<T> left, right;

        public MatchOneMaker(IMaker<T> left, IMaker<T> right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public ITokenMatching<T> TryMake(ITokenMatching elementToken)
        {
            var leftRes = left.TryMake(elementToken);
            var rightRes = right.TryMake(elementToken);

            if (leftRes.HasValue && rightRes.HasValue) {
                throw new Exception("both should not match");
            }

            if (leftRes.HasValue) {
                return leftRes;
            }

            if (rightRes.HasValue) {
                return rightRes;
            }

            // the choice of left or right is meaningless 
            return leftRes;
        }
    }

    internal class MatchOneMaker : IMaker
    {
        private readonly IMaker left, right;

        public MatchOneMaker(IMaker left, IMaker right)
        {
            this.left = left ?? throw new ArgumentNullException(nameof(left));
            this.right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public ITokenMatching TryMake(ITokenMatching elementToken)
        {
            var leftRes = left.TryMake(elementToken);
            var rightRes = right.TryMake(elementToken);

            if (!leftRes.IsNotMatch && !rightRes.IsNotMatch)
            {
                throw new Exception("both should not match");
            }

            if (!leftRes.IsNotMatch)
            {
                return leftRes;
            }

            if (!rightRes.IsNotMatch)
            {
                return rightRes;
            }

            // the choice of left or right is meaningless 
            return leftRes;
        }
    }
}
