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
using Tac.SemanticModel;
using Tac.Frontend.New.CrzayNamespace;
using Tac.Model.Elements;

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

    internal class LiteralMaker : IMaker<AtomicToken>
    {
        private readonly string s;

        public LiteralMaker(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<AtomicToken> TryMake(IMatchedTokenMatching elementToken)
        {
            var index = elementToken.EndIndex;

            if (elementToken.AllTokens.Count > index &&
                elementToken.AllTokens[index].Is1(out var v1) && v1.SafeIs(out AtomicToken first) &&
                first.Item == s)
            {
                return TokenMatching<AtomicToken>.MakeMatch(elementToken, first, index + 1);
            }

            return TokenMatching<AtomicToken>.MakeNotMatch(elementToken.Context);
        }

    }

    internal class NameMaker : IMaker<AtomicToken>
    {
        public ITokenMatching<AtomicToken> TryMake(IMatchedTokenMatching elementToken)
        {
            var index = elementToken.EndIndex;

            if (elementToken.AllTokens.Count > index &&
                elementToken.AllTokens[index].Is1(out var v1) && v1.SafeIs(out AtomicToken first) &&
                !double.TryParse(first.Item, out var _) &&
                IsNotKeyWord(first.Item))
            {
                return TokenMatching<AtomicToken>.MakeMatch(elementToken, first, index + 1);
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
                self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out AtomicToken first) &&
                !double.TryParse(first.Item, out var _) &&
                IsNotKeyWord(first.Item))
            {

                var at = TokenMatching<NameKey>.MakeStart(self.AllTokens, self.Context, index+1);
                var match = new GenericNMaker().TryMake(at);
                if (match.SafeIs(out IMatchedTokenMatching<IKey[]> mathced))
                {

                    return TokenMatching<IKey>.MakeMatch(
                        self,
                        new GenericNameKey(new NameKey(first.Item), mathced.Value.Select(x=>OrType.Make<IKey,IError>(x)).ToArray()),
                        mathced.EndIndex);
                }

                return TokenMatching<IKey>.MakeMatch(self, new NameKey(first.Item),  index+1);
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
                self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out AtomicToken first) &&
                double.TryParse(first.Item, out var res))
            {
                return TokenMatching<double>.MakeMatch(self, res, index +1);
            }

            return TokenMatching<double>.MakeNotMatch(self.Context);
        }
    }

    internal class DoneMaker : IMaker
    {
        public ITokenMatching TryMake(ITokenMatching self)
        {
            if (self.SafeIs(out IMatchedTokenMatching matched) && matched.AllTokens.Count >= matched.EndIndex )
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
                self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out CurleyBracketToken first))
            {
                return TokenMatching<CurleyBracketToken>.MakeMatch(self, first, index + 1);
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
                           self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out SquareBacketToken typeParameters) &&
                               typeParameters.Tokens.All(x => x.SafeIs(out LineToken line) &&
                                   line.Tokens.Count == 1 &&
                                   line.Tokens.ElementAt(0).SafeIs(out AtomicToken _)))
            {
                return TokenMatching<string[]>.MakeMatch(self,
                    typeParameters.Tokens.Select(x => x.CastTo<LineToken>().Tokens.Single().CastTo<AtomicToken>().Item).ToArray(), 
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
                self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out SquareBacketToken typeParameters) &&
                typeParameters.Tokens.All(x => x.SafeIs(out LineToken lt) && lt.Tokens.First().SafeIs(out AtomicToken _)) &&
                TryToToken(out var res))
            {
                return TokenMatching<IKey[]>.MakeMatch(
                    self,
                    res!,
                    index + 1);
            }

            return TokenMatching<IKey[]>.MakeNotMatch(self.Context);

            bool TryToToken(out IKey[]? typeSourcesInner)
            {
                var typeSourcesBuilding = new List<IKey>();
                foreach (var elementToken in typeParameters.Tokens.OfType<LineToken>())
                {
                    var matcher = TokenMatching<object>.MakeStart(elementToken.Tokens.Select(x=> OrType.Make<IToken,ISetUp>(x)).ToArray(), self.Context,0);
                    IKey? typeSource = null;
                    if (matcher
                            .Has(new TypeNameMaker(), out typeSource)
                            .Has(new DoneMaker()).SafeIs(out IMatchedTokenMatching<IKey?> _))
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

    internal class BinaryOperationMatcher : IMaker<(ISetUp lhs, ISetUp rhs)>
    {
        private readonly string s;

        public BinaryOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(ISetUp, ISetUp)> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count - 2 > index &&
                    self.AllTokens[index+1].Is1(out var v1) &&  v1.SafeIs(out AtomicToken op) &&
                    op.Item == s &&
                    self.AllTokens[index].Is2(out var lhs) &&
                    self.AllTokens[index+2].Is2(out var rhs))
            {
                return TokenMatching<(ISetUp, ISetUp)>.MakeMatch(
                    self,
                    (lhs,  rhs),
                    index + 3);
            }

            return TokenMatching<(ISetUp, ISetUp)>.MakeNotMatch(self.Context);
        }
    }

    internal class BinaryTypeOperationMatcher : IMaker<(ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> lhs, AtomicToken token, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference> rhs)>
    {
        private readonly string s;

        public BinaryTypeOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<(ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>, AtomicToken, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>)> TryMake(IMatchedTokenMatching self)
        {
            if (self.Has(new TypeMakerNoOp())
                    .Has(new LiteralMaker(s))
                    .Has(new TypeMaker()).SafeIs(out IMatchedTokenMatching<ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>, AtomicToken, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>> matched))
            {
                return TokenMatching<(ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>, AtomicToken, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>)>.MakeMatch(
                    self,
                    (matched.Value1, matched.Value2, matched.Value3),
                    matched.EndIndex);
            }


            return TokenMatching<(ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>, AtomicToken, ISetUp<IBox<IFrontendType<IVerifiableType>>, Tpn.TypeProblem2.TypeReference>)>.MakeNotMatch(self.Context);
        }
    }


    internal class TrailingOperationMatcher : IMaker<ISetUp>
    {
        private readonly string s;

        public TrailingOperationMatcher(string s)
        {
            this.s = s ?? throw new ArgumentNullException(nameof(s));
        }

        public ITokenMatching<ISetUp> TryMake(IMatchedTokenMatching self)
        {
            var index = self.EndIndex;

            if (self.AllTokens.Count -1 > index &&
                    self.AllTokens[index+1].Is1(out var v1) && 
                    v1.SafeIs(out AtomicToken op) &&
                    op.Item == s &&
                    self.AllTokens[index].Is2(out var setUp))
            {
                return TokenMatching<ISetUp>.MakeMatch(self, setUp, index + 2);
            }


            return TokenMatching<ISetUp>.MakeNotMatch(self.Context);
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

            if (self.AllTokens.Count > index && self.AllTokens[index].Is1(out var v1) && v1.SafeIs(out AtomicToken first) &&
                                first.Item == word)
            {
                return TokenMatching<AtomicToken>.MakeMatch(self, first, index + 1);
            }
            return TokenMatching<AtomicToken>.MakeNotMatch(self.Context);
        }
    }
}
